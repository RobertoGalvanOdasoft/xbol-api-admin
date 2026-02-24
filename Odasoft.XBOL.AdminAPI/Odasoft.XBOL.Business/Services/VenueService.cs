using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Data.Repositories;
using Odasoft.XBOL.DTO;

namespace Odasoft.XBOL.Business.Services
{
    public class VenueService
    {
        private readonly VenueRepository _venueRepository;

        public VenueService(VenueRepository venueRepository)
        {
            _venueRepository = venueRepository;
        }

        public async Task<IList<VenueListItemDTO>> GetVenueListAsync()
        {
            return await _venueRepository.GetVenueListAsync();
        }

        public async Task<IList<ListItem>> GetVenueCatalogAsync()
        {
            return await _venueRepository.Get()
                            .AsNoTracking()
                            .Where(v => v.IsActive)
                            .Select(v => new ListItem
                            {
                                Id = v.Id,
                                Name = v.Name
                            }).ToListAsync();
        }
    }
}
