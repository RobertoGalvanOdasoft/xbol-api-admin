using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Data.Repositories
{
    public class EventScheduleRepository(XBOLDbContext dbContext) : BaseRepository<EventSchedule>(dbContext)
    {
    }
}
