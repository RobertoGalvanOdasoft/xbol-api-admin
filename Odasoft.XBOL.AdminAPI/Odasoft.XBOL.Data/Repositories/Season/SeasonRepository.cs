using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Commons.Enums;
using Odasoft.XBOL.DTO;
using Odasoft.XBOL.DTO.QueryParams;
using Odasoft.XBOL.DTO.Response;
using Odasoft.XBOL.DTO.Results;

namespace Odasoft.XBOL.Data.Repositories.Season
{
    public class SeasonRepository(XBOLDbContext dbContext) : BaseRepository<Models.Season>(dbContext)
    {
        public async Task<PagedResponse<SeasonListItem>> GetSeasonsAsync(SeasonsQueryParams queryParams)
        {
            var query = DbSet.AsNoTracking().Where(s => s.DeletedAt == null).AsQueryable();

            if (queryParams.Status.HasValue)
            {
                query = query.Where(s => s.Status == queryParams.Status.Value);
            }

            if (queryParams.Period.HasValue)
            {
                var now = DateTimeOffset.UtcNow;
                query = queryParams.Period.Value switch
                {
                    SeasonPeriod.CurrentSeason => query.Where(s => s.StartDate <= now && s.EndDate > now),
                    SeasonPeriod.PastSeason => query.Where(s => s.EndDate <= now),
                    _ => query
                };
            }

            if (queryParams.StartDate.HasValue)
            {
                var startDateUtc = queryParams.StartDate.Value.ToUniversalTime();
                query = query.Where(s => s.StartDate >= startDateUtc);
            }

            if (queryParams.EndDate.HasValue)
            {
                var endDateUtc = queryParams.EndDate.Value.ToUniversalTime();
                query = query.Where(s => s.EndDate <= endDateUtc);
            }

            if (!string.IsNullOrEmpty(queryParams.SearchTerm))
            {
                query = query.Where(s => EF.Functions.ILike(s.Name, $"%{queryParams.SearchTerm}%"));
            }

            var totalCount = await query.CountAsync();

            query = queryParams.SortBy?.ToLowerInvariant() switch
            {
                "name" => queryParams.Descending
                    ? query.OrderByDescending(s => s.Name).ThenByDescending(s => s.Id)
                    : query.OrderBy(s => s.Name).ThenByDescending(s => s.Id),
                "startdate" => queryParams.Descending
                    ? query.OrderByDescending(s => s.StartDate).ThenByDescending(s => s.Id)
                    : query.OrderBy(s => s.StartDate).ThenByDescending(s => s.Id),
                "enddate" => queryParams.Descending
                    ? query.OrderByDescending(s => s.EndDate).ThenByDescending(s => s.Id)
                    : query.OrderBy(s => s.EndDate).ThenByDescending(s => s.Id),
                _ => queryParams.Descending
                    ? query.OrderByDescending(s => s.StartDate).ThenByDescending(s => s.Id)
                    : query.OrderBy(s => s.StartDate).ThenByDescending(s => s.Id)
            };

            var items = await query
                .Skip((queryParams.Page - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .Select(s => new SeasonListItem
                {
                    Id = s.Id,
                    Code = s.Code,
                    Name = s.Name,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    BannerImageUrl = s.BannerImageUrl,
                    PosterImageUrl = s.PosterImageUrl
                })
                .ToListAsync();

            return new PagedResponse<SeasonListItem>
            {
                Items = items,
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };
        }

        public async Task<long?> GetSeasonIdByExternalSeasonKeyAsync(string seasonKey)
        {
            var season = await DbContext.Set<Models.Season>()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.ExternalSeasonKey == seasonKey);

            return season?.Id;
        }

        public async Task<SeasonResult?> GetSeasonByIdAsync(long id)
        {
            var seatsPrice = await dbContext.SeasonSeats
                .AsNoTracking()
                .Where(ss => ss.SeasonSection.SeasonId == id)
                .Where(ss => ss.PriceOverride != null)
                .GroupBy(ss => ss.PriceOverride)
                .Select(g => new SeatsIoPriceDTO
                {
                    Category = null,
                    Objects = g.Select(x => x.ExternalSeatObjectKey).ToArray(),
                    OriginalPrice = null,
                    Price = g.Key ?? 0m,
                    Fee = null
                })
                .ToListAsync();

            var categoriesPrice = await dbContext.SeasonSections
                .AsNoTracking()
                .Where(ss => ss.SeasonId == id)
                .Where(ss => ss.BaseSection.BaseZone.ExternalZoneKey != null)
                .GroupBy(ss => ss.BaseSection.BaseZone.ExternalZoneKey)
                .Select(g => new SeatsIoPriceDTO
                {
                    Category = g.Key,
                    Objects = null,
                    OriginalPrice = null,
                    Price = g.Min(ss => ss.Price) ?? 0,
                    Fee = null
                })
                .ToListAsync();

            var prices = seatsPrice
                .Concat(categoriesPrice)
                .ToList();

            return await DbSet
                .AsNoTracking()
                .Where(s => s.Id == id && s.DeletedAt == null)
                .Select(s => new SeasonResult
                {
                    Id = s.Id,
                    Name = s.Name,
                    Code = s.Code,
                    Description = s.Description,
                    BannerImageUrl = s.BannerImageUrl,
                    PosterImageUrl = s.PosterImageUrl,
                    LandingUrl = s.LandingUrl,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    PublishedDate = s.PublishedDate,
                    OnSaleDate = s.OnSaleDate,
                    PreSaleDate = s.PreSaleDate,
                    OffSaleDate = s.OffSaleDate,
                    Status = s.Status,
                    ExternalSeasonKey = s.ExternalSeasonKey,
                    Venue = DbContext.Set<Models.Event>()
                        .Where(e => e.SeasonId == s.Id)
                        .Select(e => e.VenueMap.Name)
                        .FirstOrDefault(),
                    Prices = prices
                })
                .FirstOrDefaultAsync();
        }
    }
}
