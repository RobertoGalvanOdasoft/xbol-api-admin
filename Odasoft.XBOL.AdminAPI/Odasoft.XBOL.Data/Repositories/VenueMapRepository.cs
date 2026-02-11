using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.DTO;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Data.Repositories
{
    public class VenueMapRepository(XBOLDbContext dbContext) : BaseRepository<VenueMap>(dbContext)
    {
        public async Task<IList<VenueMapListItemDTO>> GetVenueMapListAsync()
        {
            return await DbSet
                .AsNoTracking()
                .Select(vm => new VenueMapListItemDTO
                {
                    Id = vm.Id,
                    VenueId = vm.VenueId,
                    Name = vm.Name,
                    ExternalMapKey = vm.ExternalMapKey
                })
                .ToListAsync();
        }

        public async Task<VenueMapListItemDTO?> GetVenueMapByIdAsync(long id)
        {
            return await DbSet
                .AsNoTracking()
                .Where(vm => vm.Id == id)
                .Select(vm => new VenueMapListItemDTO
                {
                    Id = vm.Id,
                    VenueId = vm.VenueId,
                    Name = vm.Name,
                    ExternalMapKey = vm.ExternalMapKey
                })
                .FirstOrDefaultAsync();
        }
    }
}
