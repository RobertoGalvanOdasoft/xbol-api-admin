using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Data.Repositories
{
    public class VenueMapRepository(XBOLDbContext dbContext) : BaseRepository<VenueMap>(dbContext)
    {
    }
}
