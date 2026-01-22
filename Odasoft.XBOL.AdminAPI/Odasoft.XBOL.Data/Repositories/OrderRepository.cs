using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Data.Repositories
{
    public class OrderRepository(XBOLDbContext dbContext) : BaseRepository<Order>(dbContext)
    {
    }
}
