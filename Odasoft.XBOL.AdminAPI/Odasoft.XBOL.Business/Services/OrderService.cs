using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Odasoft.XBOL.Commons.Requests.Filters;
using Odasoft.XBOL.Commons.Responses;
using Odasoft.XBOL.Data.Repositories;
using Odasoft.XBOL.Data.Repositories.Client;
using Odasoft.XBOL.Data.Repositories.Order;
using Odasoft.XBOL.Data.Repositories.Season;
using Odasoft.XBOL.DTO.Requests;
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
                var season = await _seasonPassRepository.Get(x => x.Season.ExternalSeasonKey == request.SeasonKey)
                                    .Include(x => x.Season)
                                    .FirstAsync();

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
                    OrderType = Enums.OrderType.Ticket,
                    PayformType = Enums.PayformType.BoxOffice,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = Guid.Empty,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = Guid.Empty,
                    Items = [.. seasonPasses.Select(x => new OrderItem
                    {
                        ItemType = Enums.ItemType.Ticket,
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

        private async Task<Client> CreateClientAsync(ClientInfoRequest clientInfo)
        {
            var client = new Client
            {
                Email = clientInfo.Email,
                CountryPhoneCode = clientInfo.CountryPhoneISO,
                PhoneNumber = clientInfo.PhoneNumber,
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
    }
}
