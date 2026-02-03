using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Commons.Enums;
using Odasoft.XBOL.Commons.Extensions;
using Odasoft.XBOL.Commons.Requests.Filters;
using Odasoft.XBOL.DTO.Helpers;
using Odasoft.XBOL.DTO.Requests;
using Odasoft.XBOL.Models;
using XBOL.Admin.Core.DTO;

namespace Odasoft.XBOL.Data.Repositories.Order
{
    public class OrderRepository(XBOLDbContext dbContext) : BaseRepository<Models.Order>(dbContext)
    {
        public async Task<(List<OrderListItem> Items, int TotalCount)> GetOrderListAsync(
            OrderListFilters filters)
        {
            var latestSeason = await DbContext.Set<Models.Season>()
                                        .OrderByDescending(x => x.StartDate)
                                        .Select(x => new { x.Id, x.PreviousSeasonId })
                                        .FirstOrDefaultAsync();

            if (!latestSeason.PreviousSeasonId.HasValue && filters.RenovationMode)
                return (new List<OrderListItem>(), 0);

            List<long> seasonIds =
             filters.RenovationMode && latestSeason.PreviousSeasonId is long previousId
                 ? [previousId]
                 : [latestSeason.Id];

            filters.TextFilter = string.IsNullOrWhiteSpace(filters.TextFilter) ? null : filters.TextFilter;

            List<long> previousOrdersIds = await DbContext.Set<Models.Order>().Where(o => o.Tickets.Any(t => t.SeasonPassEventTicket.SeasonPass.SeasonId == latestSeason.Id) && o.RelatedOrderId.HasValue).Select(x => x.RelatedOrderId.Value).ToListAsync();

            var query = DbContext.Set<Models.Order>().Where(o =>
                o.Tickets.Any(t => seasonIds.Any(s => s == t.SeasonPassEventTicket.SeasonPass.SeasonId)) &&
                !previousOrdersIds.Any(s => s == o.Id) &&
                (filters.RenewalTypes == null || filters.RenewalTypes.Count == 0 ||
                    (filters.RenewalTypes.Any(x =>
                        (x == SeasonPassRenewalType.New && !o.RelatedOrderId.HasValue) ||
                        (x == SeasonPassRenewalType.Renewal && o.RelatedOrderId.HasValue)))) &&
                (filters.TextFilter == null ||
                    (
                        (o.RelatedOrder != null && o.RelatedOrder.Reference.Contains(filters.TextFilter)) ||
                        o.Client.Email.Contains(filters.TextFilter) ||
                        o.Client.PhoneNumber.Contains(filters.TextFilter) ||
                        o.Reference.Contains(filters.TextFilter) ||
                        o.Tickets.Any(t => t.EventSeat.ExternalSeatObjectKey.Contains(filters.TextFilter))
                    )
                )
            );

            //sort
            query = filters.SortDesc == null
                ? query.OrderByDescending(x => x.Id)
                : filters.SortDesc.Value ? query.OrderByDescending(x => x.Reference) : query.OrderBy(x => x.Reference);

            //pagination
            int totalCount = await query.CountAsync();
            var skip = (filters.Page - 1) * filters.PageSize;

            List<OrderListItem> orders = await query.Select(o => new OrderListItem
            {
                Order = o.Reference,
                Email = o.Client.Email,
                CountryPhoneISO = o.Client.User.CountryPhoneISO,
                PhoneNumber = o.Client.PhoneNumber,
                Total = o.Total,
                SeatCount = o.Tickets.GroupBy(x => x.SeasonPassEventTicket.SeasonPassId).Count(),
                RelatedOrderReference = o.RelatedOrder != null ? o.RelatedOrder.Reference : null,

                SeasonPeriod = o.Tickets.All(t =>
                    t.SeasonPassEventTicket.SeasonPass.SeasonId == filters.SeasonId)
                        ? SeasonPeriod.CurrentSeason
                        : SeasonPeriod.PastSeason,

                SeasonPasses = o.Tickets.GroupBy(x => x.SeasonPassEventTicket.SeasonPassId).Select(g => g.Select(t => new SeasonPassItem
                {
                    SeasonPassId = t.SeasonPassEventTicket.SeasonPassId,
                    SeatCode = t.EventSeat.ExternalSeatObjectKey,
                    Price = t.PricePaid,
                    CategoryEnum = t.EventSeat.BaseSeat.SeatType,
                    Status = t.SeasonPassEventTicket.SeasonPass.Status,
                    SuspendedReason = t.SeasonPassEventTicket.SeasonPass.SuspendedReason,
                    SuspendedOtherReason = t.SeasonPassEventTicket.SeasonPass.SuspendedOtherReason,
                }).First()).ToList()
            })
            .Skip(skip)
            .Take(filters.PageSize)
            .ToListAsync();

            foreach (var seasonPass in orders.SelectMany(o => o.SeasonPasses))
            {
                seasonPass.Category = seasonPass.CategoryEnum.GetDescription();
            }

            return (orders, totalCount);
        }

        //AQUI LISTADO DE ASIENTOS
        public async Task<ClientSeasonEvent> GetClientSeasonEventByOrderReferenceAsync(string orderReference)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            ClientSeasonEvent? result = new();

            result = await DbContext.Set<Models.Order>()
                 .AsNoTracking()
                .Where(o => o.Reference == orderReference)
                .Select(o => new ClientSeasonEvent
                {
                    ClientContact = new ClientContactRequest
                    {
                        CountryPhoneISO = o.Client.User!.CountryPhoneISO ?? string.Empty,
                        PhoneNumber = o.Client.PhoneNumber ?? string.Empty,
                        Email = o.Client.Email ?? string.Empty,
                        Name = !string.IsNullOrWhiteSpace(o.Client.BusinessName)
                                    ? o.Client.BusinessName
                                    : o.Client.FirstName,
                        LastName = o.Client.LastName
                    },
                    Seats = o.Tickets
                        .GroupBy(t => t.EventSeat.ExternalSeatObjectKey)
                        .Select(g => g.Select(t => new SeatInfoRequest
                        {
                            SeatId = t.EventSeat.ExternalSeatObjectKey,
                            Price = t.EventSeat.PriceOverride ?? 0,
                            CategoryEnum = t.EventSeat.BaseSeat.SeatType
                        }).First())
                        .ToList(),
                    EventId = o.Tickets.OrderByDescending(t => t.EventSchedule.StartDateTime).Select(t => t.EventSchedule.EventId).FirstOrDefault(),
                    ClientId = o.ClientId,
                    RelatedOrderId = o.Id
                })
                .SingleOrDefaultAsync();

            var currentSeasonId = await DbContext.Set<Models.Order>()
             .AsNoTracking()
             .Where(o => o.Reference == orderReference)
             .SelectMany(o => o.Tickets).Select(t => t.EventSchedule.Event.SeasonId)
             .FirstOrDefaultAsync();

            long seasonIdToUseId = await DbContext.Set<Models.Season>()
                .AsNoTracking()
                .Where(s =>
                    (s.PreviousSeasonId == currentSeasonId && s.Status == SeasonStatus.Published) ||
                    (s.Id == currentSeasonId))
                .OrderByDescending(s => s.PreviousSeasonId == currentSeasonId && s.Status == SeasonStatus.Published)
                .ThenByDescending(s => s.StartDate)
                .Select(s => s.Id)
                .FirstAsync();

            var upcoming = await DbContext.Set<Models.EventSchedule>()
                .AsNoTracking()
                .Where(es => es.Event.SeasonId == seasonIdToUseId)// && es.StartDateTime >= now)
                .OrderBy(es => es.StartDateTime)
                .Select(es => new { es.ExternalEventKey, es.EventId, SeasonId = es.Event.Season.Id, SeasonKey = es.Event.Season.ExternalSeasonKey })
                .FirstOrDefaultAsync();

            result.EventKey = upcoming?.ExternalEventKey ?? string.Empty;
            result.EventId = upcoming?.EventId ?? 0;

            bool alreadyRenewed = await DbContext.Set<Models.SeasonPass>()
                .AnyAsync(sp => sp.ClientId == result.ClientId && sp.SeasonId == seasonIdToUseId);

            result.AlreadyRenewed = alreadyRenewed;
            result.CanRenovate = seasonIdToUseId != null && !alreadyRenewed;
            result.SeasonId = upcoming?.SeasonId ?? 0;
            result.SeasonKey = upcoming?.SeasonKey ?? string.Empty;

            foreach (var seat in result.Seats)
            {
                seat.Category = seat.CategoryEnum.GetDescription();
            }

            return result;
        }

        public async Task<Models.Order> BookSeasonAsync(BookSeasonRequest request)
        {
            Guid userId = Guid.Parse("00000000-0000-0000-0000-000000000000");
            DateTimeOffset now = DateTimeOffset.UtcNow;

            using var transaction = await DbContext.Database.BeginTransactionAsync();

            Models.Season? season = await DbContext.Set<Models.Season>()
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.Id == request.SeassonId);

            var seatKeys = request.Seats?.ToHashSet(StringComparer.OrdinalIgnoreCase)
                          ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            List<OrderEventSchedule> schedules = await DbContext.Set<Models.EventSchedule>()
                .AsNoTracking()
                .Where(es => es.Event.SeasonId == request.SeassonId)
                .Where(es => es.Sections.SelectMany(s => s.EventSeats)
                    .Any(seat => seatKeys.Contains(seat.ExternalSeatObjectKey)))
                .Select(es => new OrderEventSchedule
                {
                    Id = es.Id,
                    Seats = es.Sections
                        .SelectMany(s => s.EventSeats)
                        .Where(seat => seatKeys.Contains(seat.ExternalSeatObjectKey))
                        .Select(seat => new OrderSeat
                        {
                            Id = seat.Id,
                            PriceOverride = seat.PriceOverride,
                            BaseSeatId = seat.BaseSeatId,
                            EventSectionId = seat.EventSectionId,
                            DisplayName = seat.EventSection.DisplayName,
                            SeatNumber = seat.BaseSeat.SeatNumber,
                            ExternalSeatObjectKey = seat.ExternalSeatObjectKey
                        })
                        .ToList()
                })
                .ToListAsync();

            var client = await DbContext.Set<Models.Client>()
                .Include(c => c.User)
                .FirstOrDefaultAsync(c =>
                    c.Email == request.ClientContact.Email ||
                    (c.User != null &&
                     c.User.CountryPhoneISO == request.ClientContact.CountryPhoneISO &&
                     c.User.PhoneNumber == request.ClientContact.PhoneNumber)
                );

            if (client == null)
            {
                User user = new()
                {
                    Email = request.ClientContact.Email,
                    CountryPhoneISO = request.ClientContact.CountryPhoneISO,
                    PhoneNumber = request.ClientContact.PhoneNumber,
                    PhoneNumberNormalized = request.ClientContact.PhoneNumber,
                    CountryPhoneCode = "",
                    IsActive = true,
                    IsMfaEnabled = false,
                    CreatedAt = now,
                    UpdatedAt = now,
                    CreatedBy = userId,
                    UpdatedBy = userId,
                    Client = new Models.Client
                    {
                        ClientType = ClientType.Individual,
                        FirstName = request.ClientContact.Name,
                        LastName = request.ClientContact.LastName,
                        Email = request.ClientContact.Email,
                        PhoneNumber = request.ClientContact.PhoneNumber,
                        IsActive = true,
                        CreatedAt = now,
                        UpdatedAt = now,
                        CreatedBy = userId,
                        UpdatedBy = userId
                    }
                };

                DbContext.Set<User>().Add(user);
                await DbContext.SaveChangesAsync();
                client = user.Client;
            }

            List<SeasonPass> seasonPasses = [];
            List<Ticket> tickets = [];
            List<OrderItem> items = [];

            OrderEventSchedule firstSchedule = schedules[0];

            foreach (var seat in firstSchedule.Seats)
            {
                SeasonPass seasonPass = new()
                {
                    ClientId = client.Id,
                    SeasonId = season.Id,
                    Status = SeasonPassStatus.Active,
                    CreatedAt = now,
                    UpdatedAt = now,
                    CreatedBy = userId,
                    UpdatedBy = userId,
                    TrackingCode = "",
                    PrivateToken = "",
                    SeasonPassType = SeasonPassType.Full,
                    Price = 0,
                    PurchasedAt = now,
                    UserId = client.UserId,
                    BaseSeatId = seat.BaseSeatId
                };
                seasonPasses.Add(seasonPass);

                var orderItem = new OrderItem
                {
                    ItemType = ItemType.SeasonPass,
                    ItemReferenceId = seasonPass.Id,
                    Price = seat.PriceOverride ?? 0
                };
                items.Add(orderItem);
            }
            DbContext.Set<Models.SeasonPass>().AddRange(seasonPasses);
            await DbContext.SaveChangesAsync();

            foreach (var schedule in schedules)
            {
                foreach (var seat in schedule.Seats)
                {
                    SeasonPass seasonPass = seasonPasses.FirstOrDefault(x => x.BaseSeatId == seat.BaseSeatId);

                    var ticket = new Ticket
                    {
                        EventScheduleId = schedule.Id,
                        EventSectionId = seat.EventSectionId,
                        EventSeatId = seat.Id,
                        CurrentClientId = client.Id,
                        OriginalClientId = client.Id,
                        TicketCode = Guid.NewGuid().ToString("N"),
                        PrivateToken = Guid.NewGuid().ToString("N"),
                        PricePaid = seat.PriceOverride ?? 0,
                        Status = TicketStatus.Used,
                        TicketType = "SeasonPass",
                        SectionLabelSnapshot = seat.DisplayName,
                        SeatLabelSnapshot = seat.SeatNumber,
                        CreatedAt = now,
                        UpdatedAt = now,
                        CreatedBy = userId,
                        UpdatedBy = userId,
                        SeasonPassEventTicket = new SeasonPassEventTicket
                        {
                            SeasonPassId = seasonPass.Id,
                        }
                    };

                    tickets.Add(ticket);
                }
            }

            decimal subtotal = firstSchedule.Seats.Sum(s => s.PriceOverride ?? 0);
            Models.Order order = new()
            {
                ClientId = client.Id,
                UserId = client.UserId,
                Reference = $"{season?.Code ?? "SEASON"}-{Guid.NewGuid():N}".Substring(0, 20),
                SubTotal = subtotal,
                TotalFees = 0,
                TotalTaxes = 0,
                Total = subtotal,
                Status = OrderStatus.Paid,
                OrderType = OrderType.SeasonPass,
                PayformType = PayformType.Online,
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = userId,
                UpdatedBy = userId,
                RelatedOrderId = request.RelatedOrderId,
                Tickets = tickets,
                Items = items
            };

            DbContext.Set<Models.Order>().Add(order);

            await DbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return order;
        }
    }
}
