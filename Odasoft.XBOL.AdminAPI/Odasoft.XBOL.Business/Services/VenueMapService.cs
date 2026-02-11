using Odasoft.XBOL.Data.Repositories;
using Odasoft.XBOL.DTO;

namespace Odasoft.XBOL.Business.Services
{
    public class VenueMapService
    {
        private readonly VenueMapRepository _venueMapRepository;

        public VenueMapService(VenueMapRepository venueMapRepository)
        {
            _venueMapRepository = venueMapRepository;
        }

        public async Task<IList<VenueMapListItemDTO>> GetVenueMapListAsync()
        {
            return await _venueMapRepository.GetVenueMapListAsync();
        }

        public async Task<VenueMapListItemDTO?> GetVenueMapByIdAsync(long id)
        {
            return await _venueMapRepository.GetVenueMapByIdAsync(id);
        }
    }
}
