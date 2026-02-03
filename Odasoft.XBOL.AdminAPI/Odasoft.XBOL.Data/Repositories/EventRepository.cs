using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Commons.Enums;
using Odasoft.XBOL.DTO;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Data.Repositories
{
    public class EventRepository(XBOLDbContext dbContext) : BaseRepository<Event>(dbContext)
    {

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
