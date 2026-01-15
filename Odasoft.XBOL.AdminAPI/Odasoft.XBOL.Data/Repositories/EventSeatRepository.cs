using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.DTO;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Data.Repositories
{
    public class EventSeatRepository(XBOLDbContext dbContext) : BaseRepository<EventSeat>(dbContext)
    {
        public async Task<IList<SeatPriceDTO>> GetEventSeatPricesAsync(long eventId)
        {
            return await dbContext.EventSeats
                .AsNoTracking()
                .Where(t1 => t1.EventSection.EventSchedule.EventId == eventId)
                .Select(t1 => new SeatPriceDTO
                {
                    SeatKey = t1.ExternalSeatObjectKey,
                    Price = t1.PriceOverride.HasValue ? t1.PriceOverride.Value : 0m,
                }).ToListAsync();
        }
    }
}