using Odasoft.XBOL.Data.Repositories.Season;
using Odasoft.XBOL.DTO;

namespace Odasoft.XBOL.Business.Services
{
    public class SeasonSeatsService(SeasonSeatRepository seasonSeatRepository)
    {
        public async Task<List<SeatPriceDTO>> GetSeatPricesBySeasonIdAsync(long seasonId)
        {
            return await seasonSeatRepository.GetSeasonSeatPricesAsync(seasonId);
        }
    }
}
