using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Data.Repositories
{
    public class VenueRepository(XBOLDbContext dbContext) : BaseRepository<Venue>(dbContext)
    {
    }
}
