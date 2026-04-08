using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Data.Repositories
{
    public class VenueAmenityRepository(XBOLDbContext dbContext) : GenericRepository<VenueAmenity>(dbContext)
    {
    }
}
