using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Data.Repositories;
using Odasoft.XBOL.DTO.Responses;

namespace Odasoft.XBOL.Business.Services
{
    public class AmenityService
    {
        public readonly AmenityRepository _amenityRepository;

        public AmenityService(AmenityRepository amenityRepository)
        {
            _amenityRepository = amenityRepository;
        }

        public async Task<List<AmenityResponse>> GetAmenitiesAsync()
        {
            return await _amenityRepository
                            .Get()
                            .AsNoTracking()
                            .Select(a => new AmenityResponse
                            {
                                Id = a.Id,
                                Name = a.Name,
                                IconIdentifier = a.IconIdentifier
                            }).ToListAsync();
        }
    }
}
