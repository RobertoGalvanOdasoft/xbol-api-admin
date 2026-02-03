using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Data.Repositories
{
    public class SuiteRepository(XBOLDbContext dbContext) : BaseRepository<Suite>(dbContext)
    {
    }
}
