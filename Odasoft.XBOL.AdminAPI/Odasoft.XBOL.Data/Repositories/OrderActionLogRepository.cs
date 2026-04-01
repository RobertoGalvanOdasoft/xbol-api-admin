namespace Odasoft.XBOL.Data.Repositories
{
    public class OrderActionLogRepository(XBOLDbContext dbContext) : BaseRepository<Models.OrderActionLog>(dbContext)
    {
    }
}
