using Odasoft.XBOL.Data.Repositories.Season;
using Odasoft.XBOL.DTO.QueryParams;
using Odasoft.XBOL.DTO.Response;
using Odasoft.XBOL.DTO.Results;

namespace Odasoft.XBOL.Business.Services
{
    public class SeasonService(SeasonRepository repository)
    {
        public async Task<PagedResponse<SeasonListItem>> GetSeasonsAsync(SeasonsQueryParams queryParams)
        {
            return await repository.GetSeasonsAsync(queryParams);
        }

        public async Task<SeasonResult?> GetSeasonByIdAsync(long id)
        {
            return await repository.GetSeasonByIdAsync(id);
        }
    }
}
