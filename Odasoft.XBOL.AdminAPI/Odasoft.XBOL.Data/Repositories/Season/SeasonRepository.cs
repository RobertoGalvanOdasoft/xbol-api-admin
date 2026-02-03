using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Commons.Enums;
using Odasoft.XBOL.Models;
using XBOL.Admin.Core.DTO;

namespace Odasoft.XBOL.Data.Repositories.Season
{
    public class SeasonRepository(XBOLDbContext dbContext) : BaseRepository<Models.Season>(dbContext)
    {
        public async Task<List<SeasonSelectorItem>> GetSeasonSelectorItemsAsync()
        {
            return await DbContext.Set<Models.Season>()
                .AsNoTracking()
                .Where(s => s.Status == SeasonStatus.Published)
                .GroupBy(s => s.PerformerId)
                .Select(g => g
                    .OrderByDescending(s => s.StartDate)
                    .ThenByDescending(s => s.Id)
                    .Select(s => new SeasonSelectorItem
                    {
                        SeasonId = s.Id,
                        Name = s.Name,
                        IsCurrent = true
                    })
                    .First())
                .ToListAsync();
        }

        public async Task<long?> GetLatestEventIdBySeasonAsync(long seasonId)
        {
            return await DbContext.Set<Event>()
                .AsNoTracking()
                .Where(e =>
                    e.SeasonId == seasonId &&
                    e.Status == EventStatus.Published)
                .OrderByDescending(e => e.CreatedAt)
                .ThenByDescending(e => e.Id)
                .Select(e => (long?)e.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<string?> GetSeasonKeyAsync(long seasonId)
        {
            return await DbContext.Set<Models.Season>()
                .AsNoTracking()
                .Where(s =>
                    s.Id == seasonId)
                .Select(s => s.ExternalSeasonKey)
                .FirstOrDefaultAsync();
        }

        public async Task<SeasonBanner?> GetSeasonBannerByEventAsync(long seasonId)
        {
            string? venueMapName = await DbContext.Set<Event>()
                .Where(e => e.SeasonId == seasonId)
                .Select(e => e.VenueMap.Name)
                .FirstOrDefaultAsync();

            var seasonBanner = await DbContext.Set<Models.Season>()
                                .Where(x => x.Id == seasonId)
                                .Select(e => new SeasonBanner
                                {
                                    ImageUrl = e.BannerImageUrl,
                                    Title = e.Name,
                                    Subtitle = e.Description,
                                    Venue = venueMapName,
                                    StartDateTime = e.StartDate,
                                    EndDateTime = e.EndDate
                                }).FirstOrDefaultAsync();

            return seasonBanner;
        }
    }
}
