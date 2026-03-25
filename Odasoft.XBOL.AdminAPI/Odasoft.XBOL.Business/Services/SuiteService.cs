using Ganss.Xss;
using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Business.Extensions;
using Odasoft.XBOL.Commons.Constants;
using Odasoft.XBOL.Data.Repositories;
using Odasoft.XBOL.DTO;
using Odasoft.XBOL.DTO.QueryParams;
using Odasoft.XBOL.DTO.Requests;
using Odasoft.XBOL.DTO.Response;
using Odasoft.XBOL.DTO.Responses;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Business.Services
{
    public class SuiteService
    {
        private readonly SuiteRepository _suiteRepository;

        public SuiteService(SuiteRepository suiteRepository)
        {
            _suiteRepository = suiteRepository;
        }

        public async Task<PagedResponse<SuiteResponse>> GetSuitesAsync(SuitesQueryParams queryParams)
        {
            var query = _suiteRepository.Get()
                            .AsNoTracking()
                            .Select(s => new SuiteResponse
                            {
                                Id = s.Id,
                                VenueId = s.SuiteLevel.VenueId,
                                VenueName = s.SuiteLevel.Venue.Name,
                                SuiteLevelId = s.SuiteLevelId,
                                SuiteLevelName = s.SuiteLevel.Name,
                                Name = s.Name,
                                SuiteType = s.SuiteType,
                                Capacity = s.Capacity,
                                Policies = s.Policies,
                                Amenities = s.Amenities
                            });

            SetSuiteLevelsFilter(ref query, queryParams.Levels);
            SetSearchTermFilter(ref query, queryParams.SearchTerm);
            SetOrder(ref query, queryParams.SortBy, queryParams.Descending);

            return await query.ToPagedResponseAsync(queryParams.Page, queryParams.PageSize);
        }

        public async Task<SuiteResponse?> GetSuiteByIdAsync(long suiteId)
        {
            Suite? suite = await _suiteRepository.GetByIdAsync(suiteId);

            if (suite == null)
            {
                return null;
            }

            return new SuiteResponse
            {
                Id = suite.Id,
                VenueId = suite.SuiteLevel.VenueId,
                VenueName = suite.SuiteLevel.Venue.Name,
                SuiteLevelId = suite.SuiteLevelId,
                SuiteLevelName = suite.SuiteLevel.Name,
                Name = suite.Name,
                SuiteType = suite.SuiteType,
                Capacity = suite.Capacity,
                Policies = suite.Policies,
                Amenities = suite.Amenities
            };
        }

        public async Task<long> CreateSuiteAsync(SuiteRequest request)
        {
            var sanitizer = new HtmlSanitizer();

            var newSuite = new Suite
            {
                SuiteLevelId = request.SuiteLevelId,
                Name = request.Name,
                SuiteType = request.SuiteType,
                Capacity = request.Capacity,
                Policies = sanitizer.Sanitize(request.Policies),
                Amenities = sanitizer.Sanitize(request.Amenities),
                CreatedAt = DateTimeOffset.UtcNow.ToUniversalTime(),
                CreatedBy = Guid.Empty,
                UpdatedAt = DateTimeOffset.UtcNow.ToUniversalTime(),
                UpdatedBy = Guid.Empty
            };

            try
            {
                await _suiteRepository.InsertAsync(newSuite);
                await _suiteRepository.CommitAsync();
            }
            catch (Exception ex)
            {
                // TODO: Implement proper logging
                Console.WriteLine($"Error creating Suite: {ex.Message}");
                return 0;
            }

            return newSuite.Id;
        }

        public async Task<bool> UpdateSuiteAsync(long suiteId, SuiteRequest request)
        {
            Suite? existingSuite = await _suiteRepository.GetByIdAsync(suiteId);

            if (existingSuite == null)
            {
                // TODO: Implement proper error handling
                Console.WriteLine($"Suite with ID {suiteId} not found.");
                return false;
            }

            var sanitizer = new HtmlSanitizer();

            existingSuite.Name = request.Name;
            existingSuite.SuiteLevelId = request.SuiteLevelId;
            existingSuite.SuiteType = request.SuiteType;
            existingSuite.Capacity = request.Capacity;
            existingSuite.Policies = sanitizer.Sanitize(request.Policies);
            existingSuite.Amenities = sanitizer.Sanitize(request.Amenities);
            existingSuite.UpdatedAt = DateTimeOffset.UtcNow.ToUniversalTime();
            existingSuite.UpdatedBy = Guid.Empty;

            try
            {
                await _suiteRepository.UpdateAsync(existingSuite);
                await _suiteRepository.CommitAsync();
            }
            catch (Exception ex)
            {
                // TODO: Implement proper logging
                Console.WriteLine($"Error updating Suite: {ex.Message}");
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteSuiteAsync(long suiteId)
        {
            Suite? existingSuite = await _suiteRepository.GetByIdAsync(suiteId);

            if (existingSuite == null)
            {
                // TODO: Implement proper error handling
                Console.WriteLine($"Suite with ID {suiteId} not found.");
                return false;
            }

            try
            {
                await _suiteRepository.HardDeleteAsync(existingSuite);
                await _suiteRepository.CommitAsync();
            }
            catch (Exception ex)
            {
                // TODO: Implement proper logging
                Console.WriteLine($"Error deleting Suite. Error: {ex.Message}");
                return false;
            }

            return true;
        }

        public async Task<List<ListItem>> GetSuiteCatalogBySuiteLevelIdAsync(long suiteLevelId)
        {
            return await _suiteRepository.Get()
                            .AsNoTracking()
                            .Where(s => s.SuiteLevelId == suiteLevelId)
                            .Select(x => new ListItem
                            {
                                Id = x.Id,
                                Name = x.Name
                            }).ToListAsync();
        }

        private void SetSuiteLevelsFilter(ref IQueryable<SuiteResponse> query, string levels)
        {
            List<string> suiteLevels = string.IsNullOrWhiteSpace(levels)
                                ? []
                                : levels
                                    .Split(',')
                                    .Select(l => l.ToLower().Trim())
                                    .ToList();

            if (suiteLevels.Any())
            {
                query = query.Where(x => suiteLevels.Contains(x.SuiteLevelName.ToLower()));
            }
        }

        private void SetSearchTermFilter(ref IQueryable<SuiteResponse> query, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return;
            }

            var lowerSearchTerm = searchTerm.ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(lowerSearchTerm)
                        || x.SuiteLevelName.ToLower().Contains(lowerSearchTerm));
        }

        private void SetOrder(ref IQueryable<SuiteResponse> query, string sortBy, bool descending)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return;
            }

            query = sortBy.ToLower() switch
            {
                QueryParamsFieldNames.SUITE_NAME => descending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
                QueryParamsFieldNames.SUITE_LEVEL => descending ? query.OrderByDescending(x => x.SuiteLevelName) : query.OrderBy(x => x.SuiteLevelName),
                _ => query
            };
        }
    }
}
