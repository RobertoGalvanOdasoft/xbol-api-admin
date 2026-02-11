using Odasoft.XBOL.Data.Repositories.Season;
using Odasoft.XBOL.DTO;

namespace Odasoft.XBOL.Business.Services
{
    public class SeasonSectionService(SeasonSectionRepository seasonSectionRepository)
    {
        public async Task<IList<ZonePriceDTO>> GetZonePricesBySeasonIdAsync(long seasonId)
        {
            return await seasonSectionRepository.GetSeasonZonePricesAsync(seasonId);
        }
    }
}
