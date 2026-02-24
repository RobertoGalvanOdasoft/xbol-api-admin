using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Data.Repositories;
using Odasoft.XBOL.DTO;
using Odasoft.XBOL.DTO.Requests;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Business.Services
{
    public class SuiteLevelService
    {
        private readonly SuiteLevelRepository _suiteLevelRepository;

        public SuiteLevelService(SuiteLevelRepository suiteLevelRepository)
        {
            _suiteLevelRepository = suiteLevelRepository;
        }

        public async Task<IList<ListItem>> GetSuiteLevelCatalogAsync()
        {
            // TODO: Why dont we have a get async in the base repository?
            return await _suiteLevelRepository.Get()
                            .AsNoTracking()
                            .Select(sl => new ListItem
                            {
                                Id = sl.Id,
                                Name = sl.Name
                            }).ToListAsync();
        }

        public async Task<bool> CreateSuiteLevelAsync(CreateSuiteLevelRequest request)
        {
            var newSuiteLevel = new SuiteLevel
            {
                VenueId = request.VenueId,
                Name = request.Name,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
                CreatedBy = Guid.Empty,
                UpdatedBy = Guid.Empty
            };

            try
            {
                await _suiteLevelRepository.InsertAsync(newSuiteLevel);
                await _suiteLevelRepository.CommitAsync();
            }
            catch (Exception ex)
            {
                // TODO: Implement proper logging
                Console.WriteLine($"Error creating SuiteLevel: {ex.Message}");
                return false;
            }

            return true;
        }

        public async Task<bool> UpdateSuiteLevelAsync(UpdateSuiteLevelRequest request)
        {
            SuiteLevel? existingSuiteLevel = await _suiteLevelRepository.GetByIdAsync(request.Id);

            if (existingSuiteLevel == null)
            {
                // TODO: Implement proper error handling
                Console.WriteLine($"SuiteLevel with ID {request.Id} not found.");
                return false;
            }

            existingSuiteLevel.Name = request.Name;
            existingSuiteLevel.UpdatedAt = DateTimeOffset.UtcNow;
            existingSuiteLevel.UpdatedBy = Guid.Empty;

            try
            {
                await _suiteLevelRepository.UpdateAsync(existingSuiteLevel);
                await _suiteLevelRepository.CommitAsync();
            }
            catch (Exception ex)
            {
                // TODO: Implement proper logging
                Console.WriteLine($"Error updating SuiteLevel: {ex.Message}");
                return false;
            }

            return false;
        }

        public async Task<bool> DeleteSuiteLevelAsync(long id)
        {
            SuiteLevel? existingSuiteLevel = await _suiteLevelRepository.GetByIdAsync(id);

            if (existingSuiteLevel == null)
            {
                // TODO: Implement proper error handling
                Console.WriteLine($"SuiteLevel with ID {id} not found.");
                return false;
            }

            try
            {
                await _suiteLevelRepository.HardDeleteAsync(existingSuiteLevel);
                await _suiteLevelRepository.CommitAsync();
            }
            catch (Exception ex)
            {
                // TODO: Implement proper logging
                Console.WriteLine($"Error deleting SuiteLevel: {ex.Message}");
                return false;
            }

            return true;
        }
    }
}
