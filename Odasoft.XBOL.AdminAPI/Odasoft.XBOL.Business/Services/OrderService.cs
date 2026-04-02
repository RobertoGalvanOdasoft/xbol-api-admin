using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Localization;
using Odasoft.XBOL.Commons.Email;
using Odasoft.XBOL.Commons.Helpers;
using Odasoft.XBOL.Commons.Requests;
using Odasoft.XBOL.Commons.Requests.Filters;
using Odasoft.XBOL.Commons.Responses;
using Odasoft.XBOL.Data.Repositories;
using Odasoft.XBOL.Data.Repositories.Client;
using Odasoft.XBOL.Data.Repositories.Order;
using Odasoft.XBOL.Data.Repositories.Season;
using Odasoft.XBOL.DTO.Requests;
using Odasoft.XBOL.DTO.Responses;
using Odasoft.XBOL.Models;
using System.Globalization;
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
        private readonly SeasonService _seasonService;
        private readonly SequenceTrackerService _sequenceTrackerService;
        private readonly IStringLocalizer<EmailResource> _emailLocalizer;

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
            SeasonService seasonService,
            SequenceTrackerService sequenceTrackerService,
            IStringLocalizer<EmailResource> emailLocalizer)
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
            _seasonService = seasonService;
            _sequenceTrackerService = sequenceTrackerService;
            _emailLocalizer = emailLocalizer;
        }

        // Move to a SalesService and rename to BookEventAsync or something like that, also we need to consider the flow for the payment,
        // we will need to create the order after payment confirmation
        public async Task<long> CreateEventOrderAsync(EventBookingRequest request)
        {
            IDbContextTransaction transaction = await _orderRepository.BeginTransactionAsync();

            try
            {
                EventSchedule schedule = await _eventScheduleRepository.Get(x => x.ExternalEventKey == request.EventKey).FirstAsync();

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
                    Reference = request.Localizer,
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

                return newOrder.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to create event order. Error: {ex.Message}");
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<long> CreateSeasonOrderAsync(SeasonBookingRequest request)
        {
            IDbContextTransaction transaction = await _orderRepository.BeginTransactionAsync();

            try
            {
                Season? season = await _seasonRepository
                                    .Get()
                                    .Where(s => s.ExternalSeasonKey == request.SeasonKey)
                                    .SingleOrDefaultAsync();

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
                    Reference = request.Localizer,
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

                return newOrder.Id;
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
                var firstItemId = order.Items.FirstOrDefault()?.ItemReferenceId;

                if (firstItemId == null)
                {
                    return null;
                }

                var seasonId = await _seasonPassRepository
                                        .Get()
                                        .AsNoTracking()
                                        .Where(sp => sp.Id == firstItemId)
                                        .Select(sp => sp.SeasonId)
                                        .FirstOrDefaultAsync();

                Season? latestSeason = await _seasonService.GetLatestSeasonAsync(seasonId);

                if (latestSeason == null)
                {
                    return null;
                }

                HashSet<string> seatsSold = await GetSeatsSoldInSeasonAsync(latestSeason.Id);

                return await GetSeasonOrderRenewalInfoAsync(order, latestSeason, seatsSold);
            }

            return await GetOrderRenewalInfoAsync(order);
        }

        public async Task<OrderInfoResponse?> GetOrderInfoByReferenceAsync(string referenceId)
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
                return await GetSeasonOrderInfoAsync(order);
            }

            return await GetOrderInfoAsync(order);
        }

        public async Task<OrderEmailModel> BuildOrderEmailModelAsync(long orderId, string toAddress, string toName, string culture = "es-MX")
        {
            var order = await _orderRepository.Get(x => x.Id == orderId)
                                .Include(o => o.Items)
                                .AsNoTracking()
                                .FirstOrDefaultAsync()
                                ?? throw new InvalidOperationException($"Order {orderId} not found");

            if (order.OrderType == Enums.OrderType.SeasonPass)
            {
                return await BuildSeasonEmailModelAsync(order, toAddress, toName, culture);
            }

            return await BuildEventEmailModelAsync(order, toAddress, toName, culture);
        }

        public async Task<CanRenewOrderResponse> CanOrderBeRenewedAsync(string referenceId)
        {
            Order? order = await _orderRepository.Get()
                                .Include(x => x.Items)
                                .AsNoTracking()
                                .Where(o => o.Reference == referenceId)
                                .SingleOrDefaultAsync();

            if (order == null)
            {
                return new() { OrderId = null, CanRenew = false, NewSeasonId = null, Reference = null };
            }

            CanRenewOrderResponse response = new() { OrderId = order.Id, CanRenew = false, NewSeasonId = null, Reference = order.Reference };

            if (order.OrderType == Enums.OrderType.SeasonPass)
            {
                var passIds = order.Items.Select(oi => oi.ItemReferenceId).ToList();

                var passData = await _seasonPassRepository.Get()
                    .Where(sp => passIds.Contains(sp.Id))
                    .Select(sp => new { sp.SeasonId, sp.TrackingCode })
                    .ToListAsync();

                if (!passData.Any())
                {
                    return response;
                }

                long originalSeasonId = passData.First().SeasonId;
                var passTrackingCodes = passData.Select(p => p.TrackingCode).ToList();

                Season? latestSeason = await _seasonService.GetLatestSeasonAsync(originalSeasonId);

                if (latestSeason == null || originalSeasonId == latestSeason.Id)
                {
                    return response;
                }

                response.NewSeasonId = latestSeason.Id;

                var soldCount = await _seasonPassRepository.Get()
                    .Where(sp => sp.SeasonId == latestSeason.Id && passTrackingCodes.Contains(sp.TrackingCode))
                    .CountAsync();

                response.CanRenew = soldCount < passTrackingCodes.Count;

                return response;
            }
            else
            {
                return new() { OrderId = order.Id, CanRenew = false, NewSeasonId = null, Reference = order.Reference };
            }
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
                    SeasonSeatId = seat.Id,
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

        private async Task<HashSet<string>> GetSeatsSoldInSeasonAsync(long seasonId)
        {
            var seats = await _seasonPassRepository.Get()
                .AsNoTracking()
                .Where(sp => sp.SeasonId == seasonId)
                .Select(sp => sp.TrackingCode)
                .ToListAsync();

            return new HashSet<string>(seats);
        }

        private async Task<OrderRenewalInfoResponse?> GetSeasonOrderRenewalInfoAsync(Order order, Season latestSeason, HashSet<string> seatsSold)
        {
            List<long> seasonPassIds = order.Items.Select(oi => oi.ItemReferenceId).ToList();

            Dictionary<long, SeasonPass> seasonPasses = await _seasonPassRepository
                                                    .Get()
                                                    .Include(sp => sp.Season)
                                                    .Include(sp => sp.SeasonSeat)
                                                        .ThenInclude(b => b.BaseSeat)
                                                        .ThenInclude(bs => bs.BaseRow)
                                                        .ThenInclude(br => br.BaseSection)
                                                        .ThenInclude(s => s.BaseZone)
                                                    .AsNoTracking()
                                                    .Where(sp => seasonPassIds.Contains(sp.Id))
                                                    .ToDictionaryAsync(sp => sp.Id);

            List<string> seatObjectKeys = seasonPasses.Values.Select(x => x.TrackingCode).ToList();

            Dictionary<string, decimal?> seatPrices = await _seasonSeatRepository
                                                            .Get()
                                                            .Include(x => x.SeasonSection)
                                                            .AsNoTracking()
                                                            .Where(x => x.SeasonSection.SeasonId == latestSeason.Id
                                                                && seatObjectKeys.Contains(x.ExternalSeatObjectKey))
                                                            .ToDictionaryAsync(x => x.ExternalSeatObjectKey, x => x.SeasonSection.Price);

            List<OrderItemResponse> orderItems = order.Items.Select(oi =>
            {
                seasonPasses.TryGetValue(oi.ItemReferenceId, out SeasonPass? pass);
                string trackingCode = pass?.TrackingCode ?? "";

                return new OrderItemResponse
                {
                    Id = oi.Id,
                    IsSeasonItem = oi.ItemType == Enums.ItemType.SeasonPass,
                    SeatObjectKey = trackingCode,
                    Zone = pass?.SeasonSeat?.BaseSeat?.BaseRow?.BaseSection?.BaseZone?.Name ?? "",
                    Section = pass?.SeasonSeat?.BaseSeat?.BaseRow?.BaseSection?.Name ?? "",
                    Row = pass?.SeasonSeat?.BaseSeat?.BaseRow?.RowLabel ?? "",
                    Seat = pass?.SeasonSeat?.BaseSeat?.SeatNumber ?? "",
                    IsSold = seatsSold.Contains(trackingCode),
                    Price = oi.Price,
                    RenewalPrice = seatPrices.GetValueOrDefault(trackingCode) ?? 0
                };
            }).ToList();

            var firstPass = seasonPasses.Values.FirstOrDefault();

            return new OrderRenewalInfoResponse
            {
                OrderId = order.Id,
                CurrentSeasonId = latestSeason.Id,
                OrderSeasonId = firstPass?.SeasonId,
                IsSeasonOrder = true,
                Items = orderItems,
                Event = firstPass?.Season?.Name ?? "",
                ClientId = order.ClientId ?? 0,
                ClientName = order.Client?.FullName ?? "",
                PhoneRegionCodeId = order.Client?.PhoneRegionCodeId ?? 0,
                DialCode = order.Client?.PhoneRegionCode?.DialCode ?? "",
                PhoneNumber = order.Client?.PhoneNumber ?? "",
                Email = order.Client?.Email ?? "",
                Neighbourhood = order.Client?.Neighborhood ?? "",
                City = order.Client?.City ?? "",
                Reference = order.Reference,
            };
        }

        private async Task<OrderRenewalInfoResponse?> GetOrderRenewalInfoAsync(Order order)
        {
            List<long> ticketIds = order.Items.Select(oi => oi.ItemReferenceId).ToList();

            Dictionary<long, Ticket> ticketDictionary = await _ticketRepository.Get()
                .Include(t => t.EventSchedule).ThenInclude(tes => tes.Event)
                .Include(t => t.EventSection).ThenInclude(es => es.BaseSection).ThenInclude(bs => bs.BaseZone)
                .Include(t => t.EventSeat).ThenInclude(es => es.BaseSeat).ThenInclude(s => s.BaseRow)
                .AsNoTracking()
                .Where(t => ticketIds.Contains(t.Id))
                .ToDictionaryAsync(t => t.Id);

            List<OrderItemResponse> orderItems = order.Items.Select(oi =>
            {
                ticketDictionary.TryGetValue(oi.ItemReferenceId, out Ticket? ticket);

                return new OrderItemResponse
                {
                    Id = oi.Id,
                    IsSeasonItem = oi.ItemType == Enums.ItemType.SeasonPass,
                    SeatObjectKey = ticket?.TicketCode ?? "",
                    Zone = ticket?.EventSection?.BaseSection?.BaseZone?.Name ?? "",
                    Section = ticket?.EventSection?.BaseSection?.Name ?? "",
                    Row = ticket?.EventSeat?.BaseSeat?.BaseRow?.RowLabel ?? "",
                    Seat = ticket?.EventSeat?.BaseSeat?.SeatNumber ?? "",
                    IsSold = true,
                    Price = oi.Price,
                    RenewalPrice = 0
                };
            }).ToList();

            var firstTicket = ticketDictionary.Values.FirstOrDefault();

            return new OrderRenewalInfoResponse
            {
                OrderId = order.Id,
                IsSeasonOrder = false,
                Items = orderItems,
                Event = firstTicket?.EventSchedule?.Event?.Name ?? "",
                ClientId = order.ClientId ?? 0,
                ClientName = order.Client?.FullName ?? "",
                PhoneRegionCodeId = order.Client?.PhoneRegionCodeId ?? 0,
                DialCode = order.Client?.PhoneRegionCode?.DialCode ?? "",
                PhoneNumber = order.Client?.PhoneNumber ?? "",
                Email = order.Client?.Email ?? "",
                Neighbourhood = order.Client?.Neighborhood ?? "",
                Reference = order.Reference
            };
        }

        private async Task<OrderInfoResponse?> GetSeasonOrderInfoAsync(Order order)
        {
            List<long> seasonPassIds = order.Items.Select(oi => oi.ItemReferenceId).ToList();

            Dictionary<long, SeasonPass> seasonPasses = await _seasonPassRepository
                                                    .Get()
                                                    .Include(sp => sp.Season)
                                                    .Include(sp => sp.SeasonSeat)
                                                        .ThenInclude(sp => sp.BaseSeat)
                                                        .ThenInclude(bs => bs.BaseRow)
                                                        .ThenInclude(br => br.BaseSection)
                                                        .ThenInclude(s => s.BaseZone)
                                                    .AsNoTracking()
                                                    .Where(sp => seasonPassIds.Contains(sp.Id))
                                                    .ToDictionaryAsync(sp => sp.Id);

            List<OrderItemResponse> orderItems = order.Items.Select(oi =>
            {
                seasonPasses.TryGetValue(oi.ItemReferenceId, out SeasonPass? seasonPass);

                return new OrderItemResponse
                {
                    Id = oi.Id,
                    IsSeasonItem = oi.ItemType == Enums.ItemType.SeasonPass,
                    SeatObjectKey = seasonPass?.TrackingCode ?? "",
                    Zone = seasonPass?.SeasonSeat?.BaseSeat?.BaseRow.BaseSection.BaseZone.Name ?? "",
                    Section = seasonPass?.SeasonSeat?.BaseSeat?.BaseRow.BaseSection.Name ?? "",
                    Row = seasonPass?.SeasonSeat?.BaseSeat?.BaseRow.RowLabel ?? "",
                    Seat = seasonPass?.SeasonSeat?.BaseSeat?.SeatNumber ?? "",
                    IsDigital = seasonPass?.IsDigital ?? true,
                    Price = seasonPass?.Price ?? 0,
                    IsCourtesy = oi.IsCourtesy,
                    IsCancelled = seasonPass?.Status == Enums.SeasonPassStatus.Cancelled
                };
            }).ToList();

            return new OrderInfoResponse
            {
                OrderId = order.Id,
                Type = order.OrderType,
                Reference = order.Reference,
                OrderDateTime = order.CreatedAt,
                ItemQuantity = orderItems?.Count ?? 0,
                Items = orderItems ?? [],
                Event = seasonPasses.Values.FirstOrDefault()?.Season.Name ?? "",
                ClientId = order.ClientId ?? 0,
                ClientName = order.Client?.FullName ?? "",
                DialCode = order.Client?.PhoneRegionCode?.DialCode ?? "",
                PhoneNumber = order.Client?.PhoneNumber ?? "",
                Email = order.Client?.Email ?? "",
                Status = order.Status,
                Total = order.Total,
                Seller = "John Doe", // TODO: Get the info from the user who made the sale
                Channel = order.PayformType,
                PaymentMethod = Enums.PaymentType.Cash // TODO: Get the info from the order's payment method
            };
        }

        private async Task<OrderInfoResponse?> GetOrderInfoAsync(Order order)
        {
            List<long> ticketIds = order.Items.Select(oi => oi.ItemReferenceId).ToList();

            Dictionary<long, Ticket> tickets = await _ticketRepository
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
                                            .ToDictionaryAsync(t => t.Id);

            List<OrderItemResponse> orderItems = order.Items.Select(oi =>
            {
                tickets.TryGetValue(oi.ItemReferenceId, out Ticket? ticket);

                return new OrderItemResponse
                {
                    Id = oi.Id,
                    IsSeasonItem = oi.ItemType == Enums.ItemType.SeasonPass,
                    SeatObjectKey = ticket?.TicketCode ?? "",
                    Zone = ticket?.EventSection.BaseSection.BaseZone.Name ?? "",
                    Section = ticket?.EventSection.BaseSection.Name ?? "",
                    Row = ticket?.EventSeat.BaseSeat.BaseRow.RowLabel ?? "",
                    Seat = ticket?.EventSeat.BaseSeat.SeatNumber ?? "",
                    IsDigital = ticket?.IsDigital ?? true,
                    Price = ticket?.PricePaid ?? 0,
                    IsCourtesy = oi.IsCourtesy,
                    IsCancelled = ticket?.Status == Enums.TicketStatus.Cancelled
                };
            }).ToList();

            return new OrderInfoResponse
            {
                OrderId = order.Id,
                Type = order.OrderType,
                Reference = order.Reference,
                OrderDateTime = order.CreatedAt,
                ItemQuantity = orderItems?.Count ?? 0,
                Items = orderItems ?? [],
                Event = tickets.Values.FirstOrDefault()?.EventSchedule.Event.Name ?? "",
                ClientId = order.ClientId ?? 0,
                ClientName = order.Client?.FullName ?? "",
                DialCode = order.Client?.PhoneRegionCode?.DialCode ?? "",
                PhoneNumber = order.Client?.PhoneNumber ?? "",
                Email = order.Client?.Email ?? "",
                Status = order.Status,
                Total = order.Total,
                Seller = "Jane Doe", // TODO: Get the info from the user who made the sale
                Channel = order.PayformType,
                PaymentMethod = Enums.PaymentType.Cash // TODO: Get the info from the order's payment method
            };
        }

        public async Task<OrderEmailModel> BuildEventEmailModelAsync(Order order, string toAddress, string toName, string culture = "es-MX")
        {
            var cultureInfo = new CultureInfo(culture);
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;

            var ticketIds = order.Items
                .Where(i => i.ItemType == Enums.ItemType.Ticket)
                .Select(i => i.ItemReferenceId)
                .ToList();

            if (ticketIds.Count == 0)
            {
                throw new InvalidOperationException($"Order {order.Id} has no tickets");
            }

            var tickets = await _ticketRepository
                                .Get(t => ticketIds.Contains(t.Id))
                                .Include(t => t.EventSchedule)
                                    .ThenInclude(es => es.Event)
                                        .ThenInclude(e => e.VenueMap)
                                            .ThenInclude(vm => vm.Venue)
                                .Include(t => t.EventSeat)
                                    .ThenInclude(es => es.BaseSeat)
                                        .ThenInclude(bs => bs.BaseRow)
                                .AsNoTracking()
                                .ToListAsync();

            var firstTicket = tickets[0];
            var schedule = firstTicket.EventSchedule;
            var @event = schedule.Event;
            var venue = @event.VenueMap.Venue;

            return new OrderEmailModel
            {
                ToAddress = toAddress,
                ToName = toName,
                Culture = culture,
                Subject = string.Format(_emailLocalizer["OrderConfirmed_Subject"].Value, order.Reference),
                EventTitle = @event.Name,
                EventImageUrl = @event.PosterImageUrl,
                OrderDetails = new OrderDetailsInfo
                {
                    OrderNumber = order.Reference,
                    Date = schedule.StartDateTime.ToString(_emailLocalizer["DateFormat"].Value, cultureInfo),
                    Time = schedule.StartDateTime.ToString("h:mm tt", cultureInfo),
                    Venue = new VenueInfo
                    {
                        Name = venue.Name,
                        Address = $"{venue.StreetAddress}, {venue.City}, {venue.State}"
                    }
                },
                Seats = [.. tickets.Select(t => new SeatInfo
                {
                    SeatKey = t.TicketCode,
                    Zone = t.SectionLabelSnapshot,
                    Row = t.EventSeat.BaseSeat.BaseRow.RowLabel,
                    Seat = t.EventSeat.BaseSeat.SeatNumber
                })],
                GoogleWalletUrl = "#",
                AppleWalletUrl = "#",
                EntryInstructions =
                [
                    _emailLocalizer["EntryInstruction_1"].Value,
                    _emailLocalizer["EntryInstruction_2"].Value,
                    _emailLocalizer["EntryInstruction_3"].Value,
                    _emailLocalizer["EntryInstruction_4"].Value,
                ],
                PromoBannerImageUrl = @event.BannerImageUrl,
                PromoBannerLinkUrl = @event.LandingUrl
            };
        }

        public async Task<OrderEmailModel> BuildSeasonEmailModelAsync(Order order, string toAddress, string toName, string culture = "es-MX")
        {
            var cultureInfo = new CultureInfo(culture);
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;

            var itemIds = order.Items
                            .Where(i => i.ItemType == Enums.ItemType.SeasonPass)
                            .Select(i => i.ItemReferenceId)
                            .ToList();

            if (itemIds.Count == 0)
            {
                throw new InvalidOperationException($"Order {order.Id} has no season pass");
            }

            var seasonPasses = await _seasonPassRepository.Get(sp => itemIds.Contains(sp.Id))
                                        .Include(sp => sp.SeasonSeat)
                                            .ThenInclude(ss => ss.BaseSeat)
                                            .ThenInclude(bs => bs.BaseRow)
                                            .ThenInclude(br => br.BaseSection)
                                        .Include(sp => sp.Season)
                                        .AsNoTracking()
                                        .ToListAsync();

            var seasonFirstEventSchedule = await _eventScheduleRepository.Get()
                                                    .Include(es => es.Event)
                                                        .ThenInclude(e => e.VenueMap)
                                                            .ThenInclude(vm => vm.Venue)
                                                    .AsNoTracking()
                                                    .Where(es => es.Event.SeasonId == seasonPasses[0].SeasonId)
                                                    .FirstAsync();

            var firstPass = seasonPasses[0];

            return new OrderEmailModel
            {
                ToAddress = toAddress,
                ToName = toName,
                Culture = culture,
                Subject = string.Format(_emailLocalizer["OrderConfirmed_Subject"].Value, order.Reference),
                EventTitle = firstPass.Season.Name,
                EventImageUrl = firstPass.Season.PosterImageUrl,
                OrderDetails = new OrderDetailsInfo
                {
                    OrderNumber = order.Reference,
                    Date = firstPass.Season.StartDate.ToString(_emailLocalizer["DateFormat"].Value, cultureInfo),
                    Time = firstPass.Season.StartDate.ToString("h:mm tt", cultureInfo),
                    Venue = new VenueInfo
                    {
                        Name = seasonFirstEventSchedule.Event.VenueMap.Venue.Name,
                        Address = $"{seasonFirstEventSchedule.Event.VenueMap.Venue.StreetAddress}, {seasonFirstEventSchedule.Event.VenueMap.Venue.City}, {seasonFirstEventSchedule.Event.VenueMap.Venue.State}"
                    }
                },
                Seats = [.. seasonPasses.Select(sp => new SeatInfo
                {
                    SeatKey = sp.TrackingCode,
                    Zone = sp.SeasonSeat.BaseSeat.BaseRow.BaseSection.Name,
                    Row = sp.SeasonSeat.BaseSeat.BaseRow.RowLabel,
                    Seat = sp.SeasonSeat.BaseSeat.SeatNumber
                })],
                GoogleWalletUrl = "#",
                AppleWalletUrl = "#",
                EntryInstructions =
                [
                    _emailLocalizer["EntryInstruction_1"].Value,
                    _emailLocalizer["EntryInstruction_2"].Value,
                    _emailLocalizer["EntryInstruction_3"].Value,
                    _emailLocalizer["EntryInstruction_4"].Value,
                ],
                PromoBannerImageUrl = firstPass.Season.BannerImageUrl,
                PromoBannerLinkUrl = firstPass.Season.LandingUrl
            };
        }
    }
}
