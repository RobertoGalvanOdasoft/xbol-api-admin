using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Data.Repositories
{
    public class SuiteLevelRepository(XBOLDbContext dbContext) : BaseRepository<SuiteLevel>(dbContext)
    {
    }
}
