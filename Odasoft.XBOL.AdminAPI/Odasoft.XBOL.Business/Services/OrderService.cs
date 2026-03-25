using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Odasoft.XBOL.Commons.Helpers;
using Odasoft.XBOL.Commons.Requests.Filters;
using Odasoft.XBOL.Commons.Responses;
using Odasoft.XBOL.Data.Repositories;
using Odasoft.XBOL.Data.Repositories.Client;
using Odasoft.XBOL.Data.Repositories.Order;
using Odasoft.XBOL.Data.Repositories.Season;
using Odasoft.XBOL.DTO.Requests;
using Odasoft.XBOL.DTO.Responses;
using Odasoft.XBOL.Models;
using XBOL.Admin.Core.DTO;
using Enums = Odasoft.XBOL.Commons.Enums;

namespace Odasoft.XBOL.Business.Services
{
    public class OrderService
    {
        private readonly OrderRepository _orderRepository;
        private readonly EventScheduleRepository _eventScheduleRepository;
        private readonly TicketRepository _ticketRepository;
        private readonly EventSeatRepository _eventSeatRepository;
        private readonly SeasonRepository _seasonRepository;
        private readonly SeasonPassRepository _seasonPassRepository;
        private readonly SeasonSeatRepository _seasonSeatRepository;
        private readonly ClientRepository _clientRepository;

        private readonly ClientCreditTransactionService _clientCreditTransactionService;
        private readonly SequenceTrackerService _sequenceTrackerService;

        private const string EVENT_ORDER_LOCALIZER_PREFIX = "ORD-E";
        private const string SEASON_ORDER_LOCALIZER_PREFIX = "ORD-S";

        public OrderService(OrderRepository orderRepository,
            EventScheduleRepository eventScheduleRepository,
            TicketRepository ticketRepository,
            EventSeatRepository eventSeatRepository,
            SeasonRepository seasonRepository,
            SeasonPassRepository seasonPassRepository,
            SeasonSeatRepository seasonSeatRepository,
            ClientRepository clientService,
            ClientCreditTransactionService clientCreditTransactionService,
            SequenceTrackerService sequenceTrackerService)
        {
            _orderRepository = orderRepository;
            _eventScheduleRepository = eventScheduleRepository;
            _ticketRepository = ticketRepository;
            _eventSeatRepository = eventSeatRepository;
            _seasonRepository = seasonRepository;
            _seasonPassRepository = seasonPassRepository;
            _seasonSeatRepository = seasonSeatRepository;
            _clientRepository = clientService;

            _clientCreditTransactionService = clientCreditTransactionService;
            _sequenceTrackerService = sequenceTrackerService;
        }

        // Move to a SalesService and rename to BookEventAsync or something like that, also we need to consider the flow for the payment,
        // we will need to create the order after payment confirmation
        public async Task CreateEventOrderAsync(EventBookingRequest request)
        {
            IDbContextTransaction transaction = await _orderRepository.BeginTransactionAsync();

            try
            {
                EventSchedule schedule = await _eventScheduleRepository.Get(x => x.ExternalEventKey == request.EventKey).FirstAsync();

                var localizer = await _sequenceTrackerService.GenerateLocalizerAsync(EVENT_ORDER_LOCALIZER_PREFIX, schedule.EventId);

                Client client;

                if (request.ClientContact.Id.HasValue)
                {
                    client = await _clientRepository
                                    .Get()
                                    .AsNoTracking()
                                    .FirstAsync(x => x.Id == request.ClientContact.Id.Value);
                }
                else
                {
                    client = await CreateClientAsync(request.ClientContact);
                    request.ClientContact.Id = client.Id;
                }

                List<Ticket> tickets = await CreateTicketsAsync(request.Seats, schedule.EventId, client);

                var newOrder = new Order
                {
                    ClientId = client.Id,
                    UserId = client.UserId,
                    Reference = localizer,
                    Status = Enums.OrderStatus.Paid,
                    SubTotal = 0,
                    TotalFees = 0,
                    TotalTaxes = 0,
                    Total = (request.PaymentInfoRequest.IsCourtesy ?? false)
                                ? 0
                                : request.Seats.Sum(x => x.Value),
                    OrderType = Enums.OrderType.Ticket,
                    PayformType = Enums.PayformType.BoxOffice,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = Guid.Empty,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = Guid.Empty,
                    Items = [.. tickets.Select(x => new OrderItem
                    {
                        ItemType = Enums.ItemType.Ticket,
                        ItemReferenceId = x.Id,
                        Price = x.PricePaid,
                        IsCourtesy = request.PaymentInfoRequest.IsCourtesy ?? false
                    })]
                };

                await _orderRepository.InsertAsync(newOrder);
                await _orderRepository.CommitAsync();

                // We should validate in the request that the client has credit before even processing the payment
                if (request.PaymentInfoRequest.CreditAmount > 0
                    && client.ClientCreditAccount != null
                    && client.IsActive)
                {
                    await CreateClientCreditTransactionAsync(client, request.PaymentInfoRequest.CreditAmount.Value, newOrder.Reference);
                }

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to create event order. Error: {ex.Message}");
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task CreateSeasonOrderAsync(SeasonBookingRequest request)
        {
            IDbContextTransaction transaction = await _orderRepository.BeginTransactionAsync();

            try
            {
                Season? season = await _seasonRepository
                                    .Get()
                                    .Where(s => s.ExternalSeasonKey == request.SeasonKey)
                                    .SingleOrDefaultAsync();

                var localizer = await _sequenceTrackerService.GenerateLocalizerAsync(SEASON_ORDER_LOCALIZER_PREFIX, season.Id);

                Client client;

                if (request.ClientContact.Id.HasValue)
                {
                    client = await _clientRepository
                                    .Get()
                                    .AsNoTracking()
                                    .FirstAsync(x => x.Id == request.ClientContact.Id.Value);
                }
                else
                {
                    client = await CreateClientAsync(request.ClientContact);
                    request.ClientContact.Id = client.Id;
                }

                List<SeasonPass> seasonPasses = await CreateSeasonPassesAsync(request.Seats, season.Id, client);

                var newOrder = new Order
                {
                    ClientId = client.Id,
                    UserId = client.UserId,
                    Reference = localizer,
                    Status = Enums.OrderStatus.Paid,
                    SubTotal = 0,
                    TotalFees = 0,
                    TotalTaxes = 0,
                    Total = (request.PaymentInfoRequest.IsCourtesy ?? false)
                                ? 0
                                : request.Seats.Sum(x => x.Value),
                    OrderType = Enums.OrderType.SeasonPass,
                    PayformType = Enums.PayformType.BoxOffice,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = Guid.Empty,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = Guid.Empty,
                    Items = [.. seasonPasses.Select(x => new OrderItem
                    {
                        ItemType = Enums.ItemType.SeasonPass,
                        ItemReferenceId = x.Id,
                        Price = x.Price,
                        IsCourtesy = request.PaymentInfoRequest.IsCourtesy ?? false
                    })]
                };

                await _orderRepository.InsertAsync(newOrder);
                await _orderRepository.CommitAsync();

                // We should validate in the request that the client has credit before even processing the payment
                if (request.PaymentInfoRequest.CreditAmount > 0
                    && client.ClientCreditAccount != null
                    && client.IsActive)
                {
                    await CreateClientCreditTransactionAsync(client, request.PaymentInfoRequest.CreditAmount.Value, newOrder.Reference);
                }

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to create season pass order. Error: {ex.Message}");
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<PagedResponse<OrderListItem>> GetOrderListAsync(OrderListFilters filters)
        {
            filters.Page = Math.Max(filters.Page, 1);
            filters.PageSize = Math.Clamp(filters.PageSize, 1, 50);

            (List<OrderListItem> result, int totalCount) = await _orderRepository.GetOrderListAsync(filters);

            return new PagedResponse<OrderListItem>
            {
                Items = result,
                CurrentPage = filters.Page,
                PageSize = filters.PageSize,
                TotalItems = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)filters.PageSize)
            };
        }

        public async Task<ClientSeasonEvent> GetClientSeasonEventByOrderReferenceAsync(string orderReference)
        {
            return await _orderRepository.GetClientSeasonEventByOrderReferenceAsync(orderReference);
        }

        public async Task<OrderRenewalInfoResponse?> GetOrderRenawalInfoByReferenceAsync(string referenceId)
        {
            Order? order = await _orderRepository
                                    .Get()
                                    .Include(x => x.Items)
                                    .Include(x => x.Client)
                                        .ThenInclude(c => c.PhoneRegionCode)
                                    .AsNoTracking()
                                    .Where(o => o.Reference.Trim().ToLower() == referenceId.Trim().ToLower())
                                    .SingleOrDefaultAsync();

            if (order == null)
            {
                return null;
            }

            if (order.OrderType == Enums.OrderType.SeasonPass)
            {
                Season? latestSeason = await _seasonRepository
                                                .Get()
                                                .Where(s => s.DeletedAt == null)
                                                .AsNoTracking()
                                                .OrderByDescending(s => s.StartDate)
                                                .FirstOrDefaultAsync();

                if (latestSeason == null)
                {
                    return null;
                }

                List<string> seatsSold = await GetSeatsSoldInSeasonAsync(latestSeason.Id);

                return await GetSeasonOrderRenewalInfoAsync(order, latestSeason, seatsSold);
            }

            return await GetOrderRenewalInfo(order);
        }

        private async Task<Client> CreateClientAsync(ClientInfoRequest clientInfo)
        {
            // TODO: Better use the client service to create the client
            var client = new Client
            {
                Email = clientInfo.Email,
                PhoneRegionCodeId = clientInfo.PhoneRegionCodeId,
                PhoneNumber = PhoneNumberHelper.NormalizePhoneNumber(clientInfo.PhoneNumber ?? ""),
                FullName = clientInfo.FullName,
                BusinessName = clientInfo.FullName,
                ClientType = Enums.ClientType.Individual,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = Guid.Empty,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = Guid.Empty
            };

            await _clientRepository.InsertAsync(client);
            await _clientRepository.CommitAsync();

            return client;
        }

        private async Task<List<Ticket>> CreateTicketsAsync(IDictionary<string, decimal> seats, long eventId, Client client)
        {
            List<Ticket> tickets = new List<Ticket>();
            var seatKeys = seats.Keys.ToList();
            var eventSeats = await _eventSeatRepository
                                    .Get()
                                    .AsNoTracking()
                                    .Include(x => x.EventSection)
                                        .ThenInclude(x => x.EventSchedule)
                                    .Where(x => x.EventSection.EventSchedule.EventId == eventId
                                        && seatKeys.Contains(x.ExternalSeatObjectKey))
                                    .ToListAsync();

            var now = DateTimeOffset.UtcNow;

            foreach (var seat in eventSeats)
            {
                var ticket = new Ticket
                {
                    EventScheduleId = seat.EventSection.EventScheduleId,
                    EventSectionId = seat.EventSectionId,
                    EventSeatId = seat.Id,
                    OriginalClientId = client.Id,
                    CurrentClientId = client.Id,
                    TicketCode = seat.ExternalSeatObjectKey,
                    TicketType = "General Admission", // TODO: Define how to manage different ticket types
                    PrivateToken = Guid.NewGuid().ToString("N"), // TODO: Define the logic for the private token
                    PricePaid = seats[seat.ExternalSeatObjectKey],
                    Status = Enums.TicketStatus.Issued,
                    SeatLabelSnapshot = seat.ExternalSeatObjectKey,
                    SectionLabelSnapshot = seat.EventSection.DisplayName,
                    CreatedAt = now,
                    CreatedBy = Guid.Empty,
                    UpdatedAt = now,
                    UpdatedBy = Guid.Empty
                };

                await _ticketRepository.InsertAsync(ticket);
                tickets.Add(ticket);
            }

            await _ticketRepository.CommitAsync();
            return tickets;
        }

        private async Task<List<SeasonPass>> CreateSeasonPassesAsync(IDictionary<string, decimal> seats, long seasonId, Client client)
        {
            List<SeasonPass> seasonPasses = new List<SeasonPass>();
            var seatKeys = seats.Keys.ToList();
            var seasonSeats = await _seasonSeatRepository
                                    .Get()
                                    .AsNoTracking()
                                    .Include(x => x.SeasonSection)
                                    .Where(x => x.SeasonSection.SeasonId == seasonId
                                        && seatKeys.Contains(x.ExternalSeatObjectKey))
                                    .ToListAsync();

            var now = DateTimeOffset.UtcNow;

            foreach (var seat in seasonSeats)
            {
                var seasonPass = new SeasonPass
                {
                    ClientId = client.Id,
                    UserId = client.UserId,
                    SeasonId = seasonId,
                    BaseSeatId = seat.Id,
                    Price = seats[seat.ExternalSeatObjectKey],
                    PurchasedAt = now,
                    SeasonPassType = Enums.SeasonPassType.Full,
                    TrackingCode = seat.ExternalSeatObjectKey,
                    PrivateToken = Guid.NewGuid().ToString("N"), // TODO: Define the logic for the private token
                    Status = Enums.SeasonPassStatus.Active,
                    CreatedAt = now,
                    CreatedBy = Guid.Empty,
                    UpdatedAt = now,
                    UpdatedBy = Guid.Empty
                };

                await _seasonPassRepository.InsertAsync(seasonPass);
                seasonPasses.Add(seasonPass);
            }

            await _seasonPassRepository.CommitAsync();
            return seasonPasses;
        }

        private async Task CreateClientCreditTransactionAsync(Client client, decimal amount, string referenceId)
        {
            var transaction = new ClientCreditTransactionRequest
            {
                Amount = amount,
                PaymentType = Enums.PaymentType.Cash, // TODO: Define logic to determine payment type
                TransactionType = Enums.CreditTransactionType.Drawdown,
                TransactionDate = DateTimeOffset.UtcNow,
                Description = referenceId
            };

            await _clientCreditTransactionService.CreateCreditTransactionByCreditAccountIdAsync(client.ClientCreditAccount.Id, transaction);
        }

        private async Task<List<string>> GetSeatsSoldInSeasonAsync(long seasonId)
        {
            return await _seasonPassRepository
                            .Get()
                            .AsNoTracking()
                            .Where(sp => sp.SeasonId == seasonId)
                            .Select(sp => sp.TrackingCode)
                            .ToListAsync();
        }

        private async Task<OrderRenewalInfoResponse?> GetSeasonOrderRenewalInfoAsync(Order order, Season latestSeason, List<string> seatsSold)
        {
            List<long> seasonPassIds = order.Items.Select(oi => oi.ItemReferenceId).ToList();

            List<SeasonPass> seasonPasses = await _seasonPassRepository
                                                    .Get()
                                                    .Include(sp => sp.Season)
                                                    .Include(sp => sp.BaseSeat)
                                                        .ThenInclude(bs => bs.BaseRow)
                                                        .ThenInclude(br => br.BaseSection)
                                                        .ThenInclude(s => s.BaseZone)
                                                    .AsNoTracking()
                                                    .Where(sp => seasonPassIds.Contains(sp.Id))
                                                    .ToListAsync();

            List<OrderItemResponse> orderItems = order.Items.Select(oi => new OrderItemResponse
            {
                Id = oi.Id,
                IsSeasonItem = oi.ItemType == Enums.ItemType.SeasonPass,
                SeatObjectKey = seasonPasses.Where(t => t.Id == oi.ItemReferenceId).FirstOrDefault()?.TrackingCode ?? "",
                Zone = seasonPasses.Where(t => t.Id == oi.ItemReferenceId).FirstOrDefault()?.BaseSeat?.BaseRow.BaseSection.BaseZone.Name ?? "",
                Section = seasonPasses.Where(t => t.Id == oi.ItemReferenceId).FirstOrDefault()?.BaseSeat?.BaseRow.BaseSection.Name ?? "",
                Row = seasonPasses.Where(t => t.Id == oi.ItemReferenceId).FirstOrDefault()?.BaseSeat?.BaseRow.RowLabel ?? "",
                Seat = seasonPasses.Where(t => t.Id == oi.ItemReferenceId).FirstOrDefault()?.BaseSeat?.SeatNumber ?? "",
                IsSold = seatsSold.Contains(seasonPasses.Where(t => t.Id == oi.ItemReferenceId).FirstOrDefault()?.TrackingCode ?? "")
            }).ToList();

            return new OrderRenewalInfoResponse
            {
                OrderId = order.Id,
                CurrentSeasonId = latestSeason.Id,
                OrderSeasonId = seasonPasses.FirstOrDefault()?.SeasonId,
                IsSeasonOrder = order.OrderType == Enums.OrderType.SeasonPass,
                Items = orderItems,
                Event = seasonPasses.FirstOrDefault()?.Season.Name ?? "",
                ClientId = order.ClientId ?? 0,
                ClientName = order.Client?.FullName ?? "",
                PhoneRegionCodeId = order.Client?.PhoneRegionCodeId ?? 0,
                DialCode = order.Client?.PhoneRegionCode?.DialCode ?? "",
                PhoneNumber = order.Client?.PhoneNumber ?? "",
                Email = order.Client?.Email ?? "",
                Neighbourhood = order.Client?.Neighborhood ?? "",
                City = order.Client?.City ?? ""
            };
        }

        private async Task<OrderRenewalInfoResponse?> GetOrderRenewalInfo(Order order)
        {
            List<long> ticketIds = order.Items.Select(oi => oi.ItemReferenceId).ToList();

            List<Ticket> tickets = await _ticketRepository
                                            .Get()
                                            .Include(t => t.EventSchedule)
                                                .ThenInclude(tes => tes.Event)
                                            .Include(t => t.EventSection)
                                                .ThenInclude(es => es.BaseSection)
                                                .ThenInclude(bs => bs.BaseZone)
                                            .Include(t => t.EventSeat)
                                                .ThenInclude(es => es.BaseSeat)
                                                .ThenInclude(s => s.BaseRow)
                                            .AsNoTracking()
                                            .Where(t => ticketIds.Contains(t.Id))
                                            .ToListAsync();

            List<OrderItemResponse> orderItems = order.Items.Select(oi => new OrderItemResponse
            {
                Id = oi.Id,
                IsSeasonItem = oi.ItemType == Enums.ItemType.SeasonPass,
                SeatObjectKey = tickets.Where(t => t.Id == oi.ItemReferenceId).FirstOrDefault()?.TicketCode ?? "",
                Zone = tickets.Where(t => t.Id == oi.ItemReferenceId).FirstOrDefault()?.EventSection.BaseSection.BaseZone.Name ?? "",
                Section = tickets.Where(t => t.Id == oi.ItemReferenceId).FirstOrDefault()?.EventSection.BaseSection.Name ?? "",
                Row = tickets.Where(t => t.Id == oi.ItemReferenceId).FirstOrDefault()?.EventSeat.BaseSeat.BaseRow.RowLabel ?? "",
                Seat = tickets.Where(t => t.Id == oi.ItemReferenceId).FirstOrDefault()?.EventSeat.BaseSeat.SeatNumber ?? "",
                IsSold = true
            }).ToList();

            return new OrderRenewalInfoResponse
            {
                OrderId = order.Id,
                IsSeasonOrder = order.OrderType == Enums.OrderType.SeasonPass,
                Items = orderItems,
                Event = tickets.FirstOrDefault()?.EventSchedule.Event.Name ?? "",
                ClientId = order.ClientId ?? 0,
                ClientName = order.Client?.FullName ?? "",
                PhoneRegionCodeId = order.Client?.PhoneRegionCodeId ?? 0,
                DialCode = order.Client?.PhoneRegionCode?.DialCode ?? "",
                PhoneNumber = order.Client?.PhoneNumber ?? "",
                Email = order.Client?.Email ?? "",
                Neighbourhood = order.Client?.Neighborhood ?? ""
            };
        }
    }
}
