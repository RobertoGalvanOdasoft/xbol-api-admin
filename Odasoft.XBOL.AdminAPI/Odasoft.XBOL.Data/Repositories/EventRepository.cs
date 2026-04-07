using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Commons.Enums;
using Odasoft.XBOL.DTO;
using Odasoft.XBOL.DTO.Response;
using Odasoft.XBOL.DTO.Results;
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
            EventStatus? status = null,
            bool? upcoming = null)
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

            // TODO: Check if any schedule in an event is On Sale and return list of schedules
            if (upcoming.HasValue)
            {
                if (upcoming.Value)
                {
                    query = query.Where(x => x.Schedule!.EndDateTime >= DateTimeOffset.UtcNow);
                }
                else
                {
                    query = query.Where(x => x.Schedule!.EndDateTime <= DateTimeOffset.UtcNow);
                }
            }

            if (venueNames.Count > 0)
            {
                query = query.Where(x => venueNames.Contains(x.Event.VenueMap!.Venue.Name));
            }

            if (categoryList.Count > 0)
            {
                query = query.Where(x => x.Event.Categories.Any(c => categoryList.Contains(c.Name)));
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
                    ? query.OrderByDescending(x => x.Event.Categories.OrderBy(c => c.Name).Select(c => c.Name).FirstOrDefault())
                          .ThenByDescending(x => x.Event.Id)
                    : query.OrderBy(x => x.Event.Categories.OrderBy(c => c.Name).Select(c => c.Name).FirstOrDefault())
                          .ThenByDescending(x => x.Event.Id),
                "venue" => descending
                    ? query.OrderByDescending(x => x.Event.VenueMap!.Venue.Name).ThenByDescending(x => x.Event.Id)
                    : query.OrderBy(x => x.Event.VenueMap!.Venue.Name).ThenByDescending(x => x.Event.Id),
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
                    Categories = x.Event.Categories.Select(c => new EventCategoryResult
                    {
                        Id = c.Id,
                        Name = c.Name,
                        DisplayName = c.DisplayName,
                        IsActive = c.IsActive,
                    }).ToList(),
                    VenueMapId = x.Event.VenueMapId,
                    VenueName = x.Event.VenueMap!.Venue.Name,
                    ExternalEventKey = x.Schedule!.ExternalEventKey,
                    TotalSeats = x.Schedule!.Sections.Sum(s => s.TotalSeats),
                    AvailableSeats = x.Schedule!.Sections.Sum(s => s.AvailableSeats),
                    PosterImageUrl = x.Event.PosterImageUrl
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

        public async Task<PagedResponse<EventListItemDTO>> GetEventsOnSaleAsync(
            string? venues,
            string? categories,
            DateTimeOffset? startDate,
            DateTimeOffset? endDate,
            string? searchTerm,
            string? sortBy,
            bool descending,
            int page,
            int pageSize)
        {
            var eventsQuery = DbSet
                .AsNoTracking()
                .Where(e => e.Status == EventStatus.Published)
                .Select(e => new EventAggregationDTO
                {
                    Id = e.Id,
                    ScheduledStartDate = e.Schedules.Min(s => s.StartDateTime),
                    OnSaleDate = e.Schedules.Min(s => s.OnSaleDate),
                    OffSaleDate = e.Schedules.Max(s => s.OffSaleDate),
                    Name = e.Name,
                    Categories = new List<EventCategoryResult>(),
                    VenueMapId = e.VenueMapId,
                    VenueName = e.VenueMap!.Venue.Name,
                    ExternalEventKey = e.Schedules.First(s => s.ExternalEventKey != null).ExternalEventKey,
                    TotalSeats = e.Schedules.Sum(s => s.Sections.Sum(sec => sec.TotalSeats)),
                    AvailableSeats = e.Schedules.Sum(s => s.Sections.Sum(sec => sec.AvailableSeats)),
                    PosterImageUrl = e.PosterImageUrl,
                    SeasonId = e.SeasonId,
                    IsSeason = false
                });

            var seasonsQuery = DbContext.Set<Models.Season>()
                .AsNoTracking()
                .Where(s => s.Status == SeasonStatus.Published)
                .Select(s => new EventAggregationDTO
                {
                    Id = s.Id,
                    ScheduledStartDate = s.StartDate,
                    OnSaleDate = s.OnSaleDate,
                    OffSaleDate = s.OffSaleDate,
                    Name = s.Name,
                    Categories = new List<EventCategoryResult>(),
                    VenueMapId = 0,
                    VenueName = null,
                    ExternalEventKey = s.ExternalSeasonKey,
                    TotalSeats = s.SeasonSections.Sum(x => x.TotalSeats),
                    AvailableSeats = s.SeasonSections.Sum(x => x.AvailableSeats),
                    PosterImageUrl = s.PosterImageUrl,
                    SeasonId = s.Id,
                    IsSeason = true
                });

            var query = eventsQuery.Union(seasonsQuery);

            query = query.Where(x => x.OnSaleDate <= DateTimeOffset.UtcNow && x.OffSaleDate >= DateTimeOffset.UtcNow);

            var totalCount = await query.CountAsync();

            query = sortBy?.ToLowerInvariant() switch
            {
                "name" => descending
                    ? query.OrderByDescending(x => x.Name).ThenByDescending(x => x.Id)
                    : query.OrderBy(x => x.Name).ThenByDescending(x => x.Id),
                "category" => descending
                    ? query.OrderByDescending(x => DbContext.Set<Models.EventCategory>()
                          .Where(c => c.Events.Any(e => e.Id == x.Id))
                          .OrderBy(c => c.Name).Select(c => c.Name).FirstOrDefault())
                          .ThenByDescending(x => x.Id)
                    : query.OrderBy(x => DbContext.Set<Models.EventCategory>()
                          .Where(c => c.Events.Any(e => e.Id == x.Id))
                          .OrderBy(c => c.Name).Select(c => c.Name).FirstOrDefault())
                          .ThenByDescending(x => x.Id),
                "venue" => descending
                    ? query.OrderByDescending(x => x.VenueName).ThenByDescending(x => x.Id)
                    : query.OrderBy(x => x.VenueName).ThenByDescending(x => x.Id),
                _ => descending
                    ? query.OrderByDescending(x => x.ScheduledStartDate).ThenByDescending(x => x.Id)
                    : query.OrderBy(x => x.ScheduledStartDate).ThenByDescending(x => x.Id)
            };

            var items = await query
               .Skip((page - 1) * pageSize)
               .Take(pageSize)
               .Select(x => new EventListItemDTO
               {
                   Id = x.Id,
                   ScheduledStartDate = x.ScheduledStartDate,
                   Name = x.Name,
                   Categories = x.Categories,
                   VenueMapId = x.VenueMapId,
                   VenueName = x.VenueName,
                   ExternalEventKey = x.ExternalEventKey,
                   TotalSeats = x.TotalSeats,
                   AvailableSeats = x.AvailableSeats,
                   PosterImageUrl = x.PosterImageUrl,
                   IsSeason = x.IsSeason
               })
               .ToListAsync();

            var eventIds = items.Where(x => !x.IsSeason).Select(x => x.Id).ToList();
            if (eventIds.Count > 0)
            {
                var categoriesByEvent = await DbContext.Set<Models.Event>()
                    .AsNoTracking()
                    .Where(e => eventIds.Contains(e.Id))
                    .Select(e => new
                    {
                        e.Id,
                        Categories = e.Categories.Select(c => new EventCategoryResult
                        {
                            Id = c.Id,
                            Name = c.Name,
                            DisplayName = c.DisplayName,
                            IsActive = c.IsActive,
                        }).ToList()
                    })
                    .ToDictionaryAsync(x => x.Id, x => x.Categories);

                foreach (var item in items.Where(x => !x.IsSeason))
                {
                    if (categoriesByEvent.TryGetValue(item.Id, out var cats))
                        item.Categories = cats;
                }
            }

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
                .Select(e => new EventInfoDTO
                {
                    Id = e.Id,
                    Name = e.Name,
                    Subtitle = e.Subtitle,
                    Categories = e.Categories.Select(c => new EventCategoryResult
                    {
                        Id = c.Id,
                        Name = c.Name,
                        DisplayName = c.DisplayName,
                        IsActive = c.IsActive,
                    }).ToList(),
                    BannerImageUrl = e.BannerImageUrl,
                    VenueMapId = e.VenueMapId,
                    VenueName = e.VenueMap!.Venue.Name,
                    Prices = prices,
                    Schedules = e.Schedules
                        .OrderBy(s => s.StartDateTime)
                        .Select(s => new EventScheduleDTO
                        {
                            Id = s.Id,
                            StartDateTime = s.StartDateTime,
                            EndDateTime = s.EndDateTime,
                            ExternalEventKey = s.ExternalEventKey,
                            TotalSeats = s.Sections.Sum(sec => sec.TotalSeats),
                            AvailableSeats = s.Sections.Sum(sec => sec.AvailableSeats),
                            Status = s.Status
                        })
                        .ToList()
                }).FirstOrDefaultAsync();

            return result;
        }
    }
}
