using Odasoft.XBOL.Commons.Enums;
using Odasoft.XBOL.Data.Repositories.Season;
using Odasoft.XBOL.DTO.QueryParams;
using Odasoft.XBOL.DTO.Requests;
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

        public async Task<SeasonResult?> CreateSeasonAsync(CreateSeasonRequest request)
        {
            var newSeason = new Models.Season
            {
                Name = request.Name,
                Code = request.Code,
                Description = request.Description,
                BannerImageUrl = request.BannerImageUrl,
                PosterImageUrl = request.PosterImageUrl,
                LandingUrl = request.LandingUrl,
                StartDate = request.StartDate!.Value,
                EndDate = request.EndDate!.Value,
                PublishedDate = request.PublishedDate,
                OnSaleDate = request.OnSaleDate,
                PreSaleDate = request.PreSaleDate,
                OffSaleDate = request.OffSaleDate,
                ExternalSeasonKey = Guid.NewGuid().ToString(),
                Status = SeasonStatus.Draft,
                CreatedAt = DateTimeOffset.UtcNow.ToUniversalTime(),
                UpdatedAt = DateTimeOffset.UtcNow.ToUniversalTime(),
                CreatedBy = Guid.Empty,
                UpdatedBy = Guid.Empty
            };

            try
            {
                await repository.InsertAsync(newSeason);
                await repository.CommitAsync();
            }
            catch (Exception ex)
            {
                // TODO: Implement proper logging
                Console.WriteLine($"Error creating Season: {ex.Message}");
                return null;
            }

            return new SeasonResult
            {
                Id = newSeason.Id,
                Name = newSeason.Name,
                Code = newSeason.Code,
                Description = newSeason.Description,
                BannerImageUrl = newSeason.BannerImageUrl,
                PosterImageUrl = newSeason.PosterImageUrl,
                LandingUrl = newSeason.LandingUrl,
                StartDate = newSeason.StartDate,
                EndDate = newSeason.EndDate,
                PublishedDate = newSeason.PublishedDate,
                OnSaleDate = newSeason.OnSaleDate,
                PreSaleDate = newSeason.PreSaleDate,
                OffSaleDate = newSeason.OffSaleDate,
                Status = newSeason.Status,
                ExternalSeasonKey = newSeason.ExternalSeasonKey
            };
        }

        public async Task<bool> UpdateSeasonAsync(long id, UpdateSeasonRequest request)
        {
            Models.Season? existingSeason = await repository.GetByIdAsync(id);

            if (existingSeason == null)
            {
                // TODO: Implement proper error handling
                Console.WriteLine($"Season with ID {id} not found.");
                return false;
            }

            existingSeason.Name = request.Name;
            existingSeason.Code = request.Code;
            existingSeason.StartDate = request.StartDate!.Value;
            existingSeason.EndDate = request.EndDate!.Value;

            if (request.Description is not null)
            {
                existingSeason.Description = request.Description;
            }

            if (request.BannerImageUrl is not null)
            {
                existingSeason.BannerImageUrl = request.BannerImageUrl;
            }

            if (request.PosterImageUrl is not null)
            {
                existingSeason.PosterImageUrl = request.PosterImageUrl;
            }

            if (request.LandingUrl is not null)
            {
                existingSeason.LandingUrl = request.LandingUrl;
            }

            if (request.PublishedDate is not null)
            {
                existingSeason.PublishedDate = request.PublishedDate;
            }

            if (request.OnSaleDate is not null)
            {
                existingSeason.OnSaleDate = request.OnSaleDate;
            }

            if (request.PreSaleDate is not null)
            {
                existingSeason.PreSaleDate = request.PreSaleDate;
            }

            if (request.OffSaleDate is not null)
            {
                existingSeason.OffSaleDate = request.OffSaleDate;
            }

            existingSeason.UpdatedAt = DateTimeOffset.UtcNow.ToUniversalTime();
            existingSeason.UpdatedBy = Guid.Empty;

            try
            {
                await repository.UpdateAsync(existingSeason);
                await repository.CommitAsync();
            }
            catch (Exception ex)
            {
                // TODO: Implement proper logging
                Console.WriteLine($"Error updating Season: {ex.Message}");
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteSeasonAsync(long id)
        {
            Models.Season? existingSeason = await repository.GetByIdAsync(id);

            if (existingSeason == null)
            {
                // TODO: Implement proper error handling
                Console.WriteLine($"Season with ID {id} not found.");
                return false;
            }

            existingSeason.DeletedAt = DateTimeOffset.UtcNow.ToUniversalTime();
            existingSeason.UpdatedAt = DateTimeOffset.UtcNow.ToUniversalTime();
            existingSeason.UpdatedBy = Guid.Empty;

            try
            {
                await repository.UpdateAsync(existingSeason);
                await repository.CommitAsync();
            }
            catch (Exception ex)
            {
                // TODO: Implement proper logging
                Console.WriteLine($"Error deleting Season: {ex.Message}");
                return false;
            }

            return true;
        }
    }
}
