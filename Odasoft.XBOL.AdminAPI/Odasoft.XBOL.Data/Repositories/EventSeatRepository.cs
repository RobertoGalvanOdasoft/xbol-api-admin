using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.DTO;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Data.Repositories
{
    public class EventSeatRepository(XBOLDbContext dbContext) : BaseRepository<EventSeat>(dbContext)
    {
        // TODO: The default value for a seat could be a setting of an event or venue.
        private const decimal DEFAULT_PRICE = 0m;

        public async Task<List<SeatPriceDTO>> GetEventSeatPricesAsync(long eventId)
        {
            return await dbContext.EventSeats
                .AsNoTracking()
                .Where(t1 => t1.EventSection.EventSchedule.EventId == eventId)
                .Select(t1 => new SeatPriceDTO
                {
                    SeatKey = t1.ExternalSeatObjectKey,
                    Price = t1.PriceOverride.HasValue ? t1.PriceOverride.Value : DEFAULT_PRICE,
                }).ToListAsync();
        }

        public async Task<EventSeat?> GetByExternalSeatObjectKeyAsync(string key)
        {
            return await dbContext.EventSeats
                .AsNoTracking()
                .FirstOrDefaultAsync(es => es.ExternalSeatObjectKey == key);
        }
    }
}
