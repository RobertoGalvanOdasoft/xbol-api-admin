using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Commons.Enums;
using Odasoft.XBOL.DTO;
using Odasoft.XBOL.DTO.Response;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Data.Repositories
{
    public class EventRepository(XBOLDbContext dbContext) : BaseRepository<Event>(dbContext)
    {
        public async Task<PagedResponse<EventListItemDTO>> GetEventListAsync(
            string? venues,
            string? categories,
            DateTimeOffset? startDate,
            DateTimeOffset? endDate,
            string? searchTerm,
            string? sortBy,
            bool descending,
            int page,
            int pageSize,
            long? seasonId = null,
            EventStatus? status = null)
        {
            var venueNames = string.IsNullOrEmpty(venues)
                ? []
                : venues.Split(',').ToList();

            var categoryList = string.IsNullOrEmpty(categories)
                ? []
                : categories.Split(',').ToList();

            var query = DbSet
                .AsNoTracking()
                .Where(e => e.Status != EventStatus.Cancelled)
                .Select(e => new
                {
                    Event = e,
                    Schedule = e.Schedules.OrderBy(s => s.StartDateTime).FirstOrDefault()
                })
                .Where(x => x.Schedule != null);

            if (seasonId.HasValue)
            {
                query = query.Where(x => x.Event.SeasonId == seasonId);
            }

            if (status.HasValue)
            {
                query = query.Where(x => x.Event.Status == status);
            }

            if (venueNames.Count > 0)
            {
                query = query.Where(x => venueNames.Contains(x.Event.VenueMap.Venue.Name));
            }

            if (categoryList.Count > 0)
            {
                query = query.Where(x => categoryList.Contains(x.Event.Category.ToString()));
            }

            if (startDate.HasValue)
            {
                var startDateUtc = startDate.Value.ToUniversalTime();
                query = query.Where(x => x.Schedule!.StartDateTime >= startDateUtc);
            }

            if (endDate.HasValue)
            {
                var endDateUtc = endDate.Value.ToUniversalTime();
                query = query.Where(x => x.Schedule!.StartDateTime <= endDateUtc);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(x => EF.Functions.ILike(x.Event.Name, $"%{searchTerm}%"));
            }

            var totalCount = await query.CountAsync();

            query = sortBy?.ToLowerInvariant() switch
            {
                "name" => descending
                    ? query.OrderByDescending(x => x.Event.Name).ThenByDescending(x => x.Event.Id)
                    : query.OrderBy(x => x.Event.Name).ThenByDescending(x => x.Event.Id),
                "category" => descending
                    ? query.OrderByDescending(x => x.Event.Category).ThenByDescending(x => x.Event.Id)
                    : query.OrderBy(x => x.Event.Category).ThenByDescending(x => x.Event.Id),
                "venue" => descending
                    ? query.OrderByDescending(x => x.Event.VenueMap.Venue.Name).ThenByDescending(x => x.Event.Id)
                    : query.OrderBy(x => x.Event.VenueMap.Venue.Name).ThenByDescending(x => x.Event.Id),
                "createdat" => descending
                    ? query.OrderByDescending(x => x.Event.CreatedAt).ThenByDescending(x => x.Event.Id)
                    : query.OrderBy(x => x.Event.CreatedAt).ThenByDescending(x => x.Event.Id),
                _ => descending
                    ? query.OrderByDescending(x => x.Schedule!.StartDateTime).ThenByDescending(x => x.Event.Id)
                    : query.OrderBy(x => x.Schedule!.StartDateTime).ThenByDescending(x => x.Event.Id)
            };

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new EventListItemDTO
                {
                    Id = x.Event.Id,
                    ScheduledStartDate = x.Schedule!.StartDateTime,
                    Name = x.Event.Name,
                    Category = x.Event.Category.ToString(),
                    VenueMapId = x.Event.VenueMapId,
                    VenueName = x.Event.VenueMap.Venue.Name,
                    ExternalEventKey = x.Schedule.ExternalEventKey,
                    TotalSeats = x.Schedule.Sections.Sum(s => s.TotalSeats),
                    AvailableSeats = x.Schedule.Sections.Sum(s => s.AvailableSeats)
                })
                .ToListAsync();

            return new PagedResponse<EventListItemDTO>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<EventInfoDTO?> GetEventByIdAsync(long eventId)
        {
            var seatsPrice = await dbContext.EventSeats
                .AsNoTracking()
                .Where(es => es.EventSection.EventSchedule.EventId == eventId)
                .Where(es => es.PriceOverride != null)
                .GroupBy(es => es.PriceOverride)
                .Select(g => new SeatsIoPriceDTO
                {
                    Category = null,
                    Objects = g.Select(x => x.ExternalSeatObjectKey).ToArray(),
                    OriginalPrice = null,
                    Price = g.Key ?? 0m,
                    Fee = null
                })
                .ToListAsync();

            var categoriesPrice = await dbContext.EventSections
                .AsNoTracking()
                .Where(es => es.EventSchedule.EventId == eventId)
                .Where(es => es.BaseSection.BaseZone.ExternalZoneKey != null)
                .GroupBy(es => es.BaseSection.BaseZone.ExternalZoneKey)
                .Select(g => new SeatsIoPriceDTO
                {
                    Category = g.Key,
                    Objects = null,
                    OriginalPrice = null,
                    Price = g.Min(es => es.Price) ?? 0,
                    Fee = null
                })
                .ToListAsync();

            var prices = seatsPrice
                .Concat(categoriesPrice)
                .ToList();

            var result = await DbSet
                .AsNoTracking()
                .Where(e => e.Id == eventId && e.Status != EventStatus.Cancelled)
                .Select(e => new
                {
                    Event = e,
                    Schedule = e.Schedules.OrderBy(s => s.StartDateTime).FirstOrDefault()
                })
                .Select(temp => new EventInfoDTO
                {
                    Id = temp.Event.Id,
                    ScheduledStartDate = temp.Schedule != null ? temp.Schedule.StartDateTime : DateTimeOffset.MinValue,
                    Name = temp.Event.Name,
                    Category = temp.Event.Category.ToString(),
                    VenueMapId = temp.Event.VenueMapId,
                    VenueName = temp.Event.VenueMap.Venue.Name,
                    ExternalEventKey = temp.Schedule != null ? temp.Schedule.ExternalEventKey : string.Empty,
                    TotalSeats = temp.Schedule != null ? temp.Schedule.Sections.Sum(s => s.TotalSeats) : 0,
                    AvailableSeats = temp.Schedule != null ? temp.Schedule.Sections.Sum(s => s.AvailableSeats) : 0,
                    Prices = prices
                }).FirstOrDefaultAsync();

            return result;
        }
    }
}
