using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.DTO;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Data.Repositories.Season
{
    public class SeasonSeatRepository(XBOLDbContext dbContext) : BaseRepository<SeasonSeat>(dbContext)
    {
        private const decimal DEFAULT_PRICE = 0m;

        public async Task<IList<SeatPriceDTO>> GetSeasonSeatPricesAsync(long seasonId)
        {
            return await dbContext.SeasonSeats
                .AsNoTracking()
                .Where(ss => ss.SeasonSection.SeasonId == seasonId)
                .Select(ss => new SeatPriceDTO
                {
                    SeatKey = ss.ExternalSeatObjectKey,
                    Price = ss.PriceOverride.HasValue ? ss.PriceOverride.Value : DEFAULT_PRICE,
                }).ToListAsync();
        }
    }
}
