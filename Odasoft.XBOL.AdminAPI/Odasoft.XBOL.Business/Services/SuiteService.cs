using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Commons.Constants;
using Odasoft.XBOL.Data.Repositories;
using Odasoft.XBOL.DTO.QueryParams;
using Odasoft.XBOL.DTO.Requests;
using Odasoft.XBOL.DTO.Response;
using Odasoft.XBOL.DTO.Results;
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

        public async Task<PagedResponse<SuiteResult>> GetSuitesAsync(SuitesQueryParams queryParams)
        {
            var query = _suiteRepository.Get()
                            .AsNoTracking()
                            .Select(s => new SuiteResult
                            {
                                Id = s.Id,
                                SuiteLevelId = s.SuiteLevelId,
                                SuiteLevelName = s.SuiteLevel.Name,
                                Name = s.Name,
                                Seats = s.Seats
                            });

            SetSuiteLevelsFilter(ref query, queryParams.Levels);
            SetSearchTermFilter(ref query, queryParams.SearchTerm);
            SetOrder(ref query, queryParams.SortBy, queryParams.Descending);

            List<SuiteResult> suites = await query.ToListAsync();

            int totalCount = suites.Count();

            return new PagedResponse<SuiteResult>
            {
                Items = suites
                        .Skip((queryParams.Page) * queryParams.PageSize)
                        .Take(queryParams.PageSize).ToList(),
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };
        }

        public async Task<SuiteResult?> GetSuiteByIdAsync(long suiteId)
        {
            Suite? suite = await _suiteRepository.GetByIdAsync(suiteId);

            if (suite == null)
            {
                return null;
            }

            return new SuiteResult
            {
                Id = suite.Id,
                SuiteLevelId = suite.SuiteLevelId,
                Name = suite.Name,
                Seats = suite.Seats
            };
        }

        public async Task<bool> CreateSuiteAsync(CreateSuiteRequest request)
        {
            var newSuite = new Suite
            {
                SuiteLevelId = request.SuiteLevelId,
                Name = request.Name,
                Seats = request.Seats,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
                CreatedBy = Guid.Empty,
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
                return false;
            }

            return true;
        }

        public async Task<bool> UpdateSuiteAsync(UpdateSuiteRequest request)
        {
            Suite? existingSuite = await _suiteRepository.GetByIdAsync(request.Id);

            if (existingSuite == null)
            {
                // TODO: Implement proper error handling
                Console.WriteLine($"Suite with ID {request.Id} not found.");
                return false;
            }

            existingSuite.Name = request.Name;
            existingSuite.SuiteLevelId = request.SuiteLevelId;
            existingSuite.Seats = request.Seats;
            existingSuite.UpdatedAt = DateTimeOffset.UtcNow;
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
                Console.WriteLine($"Error updating Suite active status: {ex.Message}");
                return false;
            }

            return true;
        }

        private void SetSuiteLevelsFilter(ref IQueryable<SuiteResult> query, string levels)
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

        private void SetSearchTermFilter(ref IQueryable<SuiteResult> query, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm) == false)
            {
                var lowerSearchTerm = searchTerm.ToLower();
                query = query.Where(x => x.Name.ToLower().Contains(lowerSearchTerm)
                            || x.SuiteLevelName.ToLower().Contains(lowerSearchTerm));
            }
        }

        private void SetOrder(ref IQueryable<SuiteResult> query, string sortBy, bool descending)
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
