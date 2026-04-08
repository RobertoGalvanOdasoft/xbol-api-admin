using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Data.Repositories;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Business.Services
{
    public class VenueAmenityService
    {
        private readonly VenueAmenityRepository _venueAmenityRepository;

        public VenueAmenityService(VenueAmenityRepository venueAmenityRepository)
        {
            _venueAmenityRepository = venueAmenityRepository;
        }

        public async Task<List<long>> SaveVenueAmenitiesAsync(long venueId, List<long> amenityIds)
        {
            // Remove existing amenities for the venue
            List<VenueAmenity> venueAmenities = await _venueAmenityRepository
                                                        .Get()
                                                        .AsNoTracking()
                                                        .Where(va => va.VenueId == venueId)
                                                        .ToListAsync();

            foreach (var venueAmenity in venueAmenities)
            {
                await _venueAmenityRepository.HardDeleteAsync(venueAmenity);
            }

            await _venueAmenityRepository.CommitAsync();

            List<VenueAmenity> newAmenities = [];

            foreach (var amenityId in amenityIds)
            {
                VenueAmenity newVenueAmenity = new VenueAmenity
                {
                    VenueId = venueId,
                    AmenityId = amenityId
                };

                await _venueAmenityRepository.InsertAsync(newVenueAmenity);

                newAmenities.Add(newVenueAmenity);
            }

            await _venueAmenityRepository.CommitAsync();

            return newAmenities.Select(va => va.AmenityId).ToList();
        }
    }
}
