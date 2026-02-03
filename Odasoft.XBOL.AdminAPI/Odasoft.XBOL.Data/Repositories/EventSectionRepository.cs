using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.DTO;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Data.Repositories
{
    public class EventSectionRepository(XBOLDbContext dbContext) : BaseRepository<EventSection>(dbContext)
    {
        public async Task<IList<ZonePriceDTO>> GetEventZonePricesAsync(long eventId)
        {
            return await DbSet
                .Where(es => es.EventSchedule.EventId == eventId)
                .Where(es => es.BaseSection.BaseZone.ExternalZoneKey != null)
                .GroupBy(es => es.BaseSection.BaseZone.ExternalZoneKey)
                .Select(g => new ZonePriceDTO
                {
                    Category = g.Key.Value,
                    Price = g.Min(es => es.Price)
                })
                .ToListAsync();
        }
    }
}
