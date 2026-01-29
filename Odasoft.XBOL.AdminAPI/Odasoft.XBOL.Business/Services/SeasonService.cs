using Odasoft.XBOL.Data.Repositories.Season;
using XBOL.Admin.Core.DTO;

namespace Odasoft.XBOL.Business.Services
{
    public class SeasonService(SeasonRepository repository)
    {
        public async Task<List<SeasonSelectorItem>> GetSeasonSelectorItemsAsync()
        {
            List<SeasonSelectorItem> items = await repository.GetSeasonSelectorItemsAsync();
            return items;
        }

        public async Task<long?> GetLatestEventIdBySeasonAsync(long seasonId)
        {
            long? eventId = await repository.GetLatestEventIdBySeasonAsync(seasonId);
            return eventId;
        }

        public async Task<SeasonBanner?> GetSeasonBannerByEventAsync(long seasonId)
        {
            SeasonBanner? seasonBanner = await repository.GetSeasonBannerByEventAsync(seasonId);
            return seasonBanner;
        }

        public async Task<string?> GetSeasonKeyAsync(long seasonId)
        {
            string? seasonKey = await repository.GetSeasonKeyAsync(seasonId);
            return seasonKey;
        }
    }
}
