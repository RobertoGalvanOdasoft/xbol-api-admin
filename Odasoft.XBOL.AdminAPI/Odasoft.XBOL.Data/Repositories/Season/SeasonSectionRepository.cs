using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.DTO;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Data.Repositories.Season
{
    public class SeasonSectionRepository(XBOLDbContext dbContext) : BaseRepository<SeasonSection>(dbContext)
    {
        public async Task<List<ZonePriceDTO>> GetSeasonZonePricesAsync(long seasonId)
        {
            return await DbSet
                .Where(ss => ss.SeasonId == seasonId)
                .Where(ss => ss.BaseSection.BaseZone.ExternalZoneKey != null)
                .GroupBy(ss => ss.BaseSection.BaseZone.ExternalZoneKey)
                .Select(g => new ZonePriceDTO
                {
                    Category = g.Key!.Value,
                    Price = g.Min(ss => ss.Price)
                })
                .ToListAsync();
        }
    }
}
