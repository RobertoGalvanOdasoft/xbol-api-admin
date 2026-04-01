using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Data.Repositories
{
    public class EventImageRepository(XBOLDbContext dbContext) : BaseRepository<EventImage>(dbContext)
    {
    }
}
