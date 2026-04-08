using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Data.Repositories
{
    public class AmenityRepository(XBOLDbContext dbContext) : BaseRepository<Amenity>(dbContext)
    {
    }
}
