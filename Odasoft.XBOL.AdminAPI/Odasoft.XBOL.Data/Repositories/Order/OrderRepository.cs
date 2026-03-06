using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Commons.Enums;
using Odasoft.XBOL.Commons.Extensions;
using Odasoft.XBOL.Commons.Requests.Filters;
using XBOL.Admin.Core.DTO;

namespace Odasoft.XBOL.Data.Repositories.Order
{
    public class OrderRepository(XBOLDbContext dbContext) : BaseRepository<Models.Order>(dbContext)
    {
        // TODO: This method is getting too complex, consider splitting the responsibilities in multiple methods or even repositories in the future
        public async Task<(List<OrderListItem> Items, int TotalCount)> GetOrderListAsync(OrderListFilters filters)
        {
            var latestSeason = await DbContext.Set<Models.Season>()
                                        .OrderByDescending(x => x.StartDate)
                                        .Select(x => new { x.Id, x.PreviousSeasonId })
                                        .FirstOrDefaultAsync();

            if (latestSeason == default || !latestSeason.PreviousSeasonId.HasValue && filters.RenovationMode)
            {
                return (new List<OrderListItem>(), 0);
            }

            List<long> seasonIds =
             filters.RenovationMode && latestSeason.PreviousSeasonId is long previousId
                 ? [previousId]
                 : [latestSeason.Id];

            filters.TextFilter = string.IsNullOrWhiteSpace(filters.TextFilter) ? null : filters.TextFilter;

            List<long> previousOrdersIds = await DbContext
                .Set<Models.Order>()
                .Where(o => o.RelatedOrderId.HasValue
                && o.Tickets.Any(t =>
                    t.SeasonPassEventTicket != null
                    && t.SeasonPassEventTicket.SeasonPass != null
                    && t.SeasonPassEventTicket.SeasonPass.SeasonId == latestSeason.Id))
                .Select(x => x.RelatedOrderId!.Value)
                .ToListAsync();

            var query = DbContext.Set<Models.Order>().AsQueryable();

            // Base Ticket & Previous Order Filters
            query = query.Where(o =>
                o.Tickets.Any(t =>
                    t.SeasonPassEventTicket != null
                    && t.SeasonPassEventTicket.SeasonPass != null
                    && seasonIds.Contains(t.SeasonPassEventTicket.SeasonPass.SeasonId))
                && !previousOrdersIds.Contains(o.Id)
            );

            // Renewal Types
            if (filters.RenewalTypes != null && filters.RenewalTypes.Any())
            {
                bool wantsNew = filters.RenewalTypes.Contains(SeasonPassRenewalType.New);
                bool wantsRenewal = filters.RenewalTypes.Contains(SeasonPassRenewalType.Renewal);

                if (wantsNew && !wantsRenewal)
                {
                    query = query.Where(o => !o.RelatedOrderId.HasValue);
                }
                else if (wantsRenewal && !wantsNew)
                {
                    query = query.Where(o => o.RelatedOrderId.HasValue);
                }
            }

            // Text Filter
            if (!string.IsNullOrWhiteSpace(filters.TextFilter))
            {
                string search = filters.TextFilter; // Store in local variable to prevent closure warnings

                query = query.Where(o =>
                    (o.RelatedOrder != null && o.RelatedOrder.Reference != null && o.RelatedOrder.Reference.Contains(search))
                    || (o.Client != null && o.Client.Email != null && o.Client.Email.Contains(search))
                    || (o.Client != null && o.Client.PhoneNumber != null && o.Client.PhoneNumber.Contains(search))
                    || (o.Reference != null && o.Reference.Contains(search))
                    || o.Tickets.Any(t =>
                        t.EventSeat != null
                        && t.EventSeat.ExternalSeatObjectKey != null
                        && t.EventSeat.ExternalSeatObjectKey.Contains(search))
                );
            }

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
                Email = o.Client == null ? "" : o.Client.Email ?? "",
                PhoneRegionCodeId = o.Client == null
                                    ? null
                                    : o.Client.PhoneRegionCodeId,
                PhoneNumber = o.Client == null ? "" : o.Client.PhoneNumber ?? "",
                Total = o.Total,
                SeatCount = o.Tickets.GroupBy(x =>
                    x.SeasonPassEventTicket == null
                    ? 0
                    : x.SeasonPassEventTicket.SeasonPassId).Count(),
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
                        PhoneRegionCodeId = o.Client == null ? null : o.Client.PhoneRegionCodeId,
                        PhoneNumber = o.Client == null ? "" : o.Client.PhoneNumber ?? "",
                        Email = o.Client == null ? "" : o.Client.Email ?? "",
                        Name = o.Client == null ? ""
                            : (string.IsNullOrWhiteSpace(o.Client.BusinessName)
                                    ? (o.Client.FullName ?? "")
                                    : o.Client.BusinessName),
                        LastName = ""
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
                .Select(es => new
                {
                    es.ExternalEventKey,
                    es.EventId,
                    SeasonId = es.Event.Season == null
                                ? 0
                                : es.Event.Season.Id,
                    SeasonKey = es.Event.Season == null
                                ? ""
                                : es.Event.Season.ExternalSeasonKey
                }).FirstOrDefaultAsync();

            if (result is not null)
            {
                result.EventKey = upcoming?.ExternalEventKey ?? "";
                result.EventId = upcoming?.EventId ?? 0;

                bool alreadyRenewed = await DbContext.Set<Models.SeasonPass>()
                    .AnyAsync(sp => sp.ClientId == result.ClientId && sp.SeasonId == seasonIdToUseId);

                result.AlreadyRenewed = alreadyRenewed;
                result.CanRenovate = seasonIdToUseId > 0 && !alreadyRenewed;
                result.SeasonId = upcoming?.SeasonId ?? 0;
                result.SeasonKey = upcoming?.SeasonKey ?? "";

                foreach (var seat in result.Seats)
                {
                    seat.Category = seat.CategoryEnum.GetDescription();
                }
            }

            return result;
        }
    }
}
