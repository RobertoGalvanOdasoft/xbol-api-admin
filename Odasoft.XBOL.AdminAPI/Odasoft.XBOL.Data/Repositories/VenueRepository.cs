using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.DTO;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Data.Repositories
{
    public class VenueRepository(XBOLDbContext dbContext) : BaseRepository<Venue>(dbContext)
    {
        public async Task<IList<VenueListItemDTO>> GetVenueListAsync()
        {
            return await DbSet
                .AsNoTracking()
                .Where(v => v.IsActive)
                .Select(v => new VenueListItemDTO
                {
                    Id = v.Id,
                    Name = v.Name
                })
                .ToListAsync();
        }
    }
}
