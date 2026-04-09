using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Data.Repositories;
using Odasoft.XBOL.DTO;
using Odasoft.XBOL.DTO.Requests;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Business.Services
{
    public class VenueMapService
    {
        private readonly VenueMapRepository _venueMapRepository;

        public VenueMapService(VenueMapRepository venueMapRepository)
        {
            _venueMapRepository = venueMapRepository;
        }

        public async Task<IList<VenueMapResponse>> GetVenueMapListAsync()
        {
            return await _venueMapRepository
                            .Get()
                            .AsNoTracking()
                            .Where(vm => vm.IsDeleted == false)
                            .Select(vm => new VenueMapResponse
                            {
                                Id = vm.Id,
                                VenueId = vm.VenueId,
                                Name = vm.Name,
                                Capacity = vm.Capacity,
                                ExternalMapKey = vm.ExternalMapKey,
                                ThumbnailUrl = vm.ThumbnailUrl
                            }).ToListAsync();
        }

        public async Task<IList<VenueMapResponse>> GetVenueMapsByVenueAsync(long venueId)
        {
            return await _venueMapRepository
                            .Get()
                            .AsNoTracking()
                            .Where(vm => vm.IsDeleted == false
                                && vm.VenueId == venueId)
                            .Select(vm => new VenueMapResponse
                            {
                                Id = vm.Id,
                                VenueId = vm.VenueId,
                                Name = vm.Name,
                                Capacity = vm.Capacity,
                                ExternalMapKey = vm.ExternalMapKey,
                                ThumbnailUrl = vm.ThumbnailUrl
                            }).ToListAsync();
        }

        public async Task<VenueMapResponse?> GetVenueMapByIdAsync(long venueMapId)
        {
            return await _venueMapRepository
                            .Get()
                            .AsNoTracking()
                            .Where(vm => vm.Id == venueMapId
                                && vm.IsDeleted == false)
                            .Select(vm => new VenueMapResponse
                            {
                                Id = vm.Id,
                                VenueId = vm.VenueId,
                                Name = vm.Name,
                                Capacity = vm.Capacity,
                                ExternalMapKey = vm.ExternalMapKey,
                                ThumbnailUrl = vm.ThumbnailUrl
                            }).FirstOrDefaultAsync();
        }

        public async Task<bool> DeleteVenueMapAsync(long venueMapId)
        {
            try
            {
                VenueMap? venueMap = await _venueMapRepository.GetByIdAsync(venueMapId);

                if (venueMap == null || venueMap.IsDeleted)
                {
                    Console.WriteLine($"Venue Map with Id '{venueMapId}' was not found");
                    return false;
                }

                venueMap.IsDeleted = true;
                venueMap.UpdatedAt = DateTimeOffset.UtcNow.ToUniversalTime();
                venueMap.UpdatedBy = Guid.Empty;

                await _venueMapRepository.UpdateAsync(venueMap);
                await _venueMapRepository.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to delete venue map with Id '{venueMapId}'. {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateVenueMapAsync(long venueMapId, VenueMapRequest request)
        {
            try
            {
                VenueMap? venueMap = await _venueMapRepository.GetByIdAsync(venueMapId);

                if (venueMap == null || venueMap.IsDeleted)
                {
                    Console.WriteLine($"Venue map with Id '{venueMapId}' was not found.");
                    return false;
                }

                venueMap.VenueId = venueMap.VenueId;
                venueMap.Capacity = request.Capacity;
                venueMap.Name = request.Name;
                venueMap.ExternalMapKey = request.ExternalMapKey;
                venueMap.ThumbnailUrl = request.ThumbnailUrl;
                venueMap.UpdatedAt = DateTimeOffset.UtcNow.ToUniversalTime();
                venueMap.UpdatedBy = Guid.Empty;

                await _venueMapRepository.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while trying to update a Venue Map. {ex.Message}");
                return false;
            }
        }

        public async Task<long> CreateVenueMapAsync(VenueMapRequest request)
        {
            try
            {
                VenueMap venueMap = new VenueMap()
                {
                    VenueId = request.VenueId,
                    Capacity = request.Capacity,
                    Name = request.Name,
                    ExternalMapKey = request.ExternalMapKey,
                    ThumbnailUrl = request.ThumbnailUrl,
                    CreatedAt = DateTimeOffset.UtcNow.ToUniversalTime(),
                    CreatedBy = Guid.Empty,
                    UpdatedAt = DateTimeOffset.UtcNow.ToUniversalTime(),
                    UpdatedBy = Guid.Empty
                };

                await _venueMapRepository.InsertAsync(venueMap);

                await _venueMapRepository.CommitAsync();

                return venueMap.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while trying to create a Venue Map. {ex.Message}");
                return 0;
            }
        }

        public async Task<List<VenueMapResponse>> GetVenueMapCatalogByVenueIdAsync(long? venueId)
        {
            return await _venueMapRepository
                            .Get()
                            .AsNoTracking()
                            .Where(vm => vm.IsDeleted == false)
                            .Where(vm => venueId == null || vm.VenueId == venueId)
                            .Select(vm => new VenueMapResponse
                            {
                                Id = vm.Id,
                                VenueId = vm.VenueId,
                                Name = vm.Name,
                                Capacity = vm.Capacity,
                                ExternalMapKey = vm.ExternalMapKey,
                                ThumbnailUrl = vm.ThumbnailUrl
                            }).ToListAsync();
        }
    }
}
