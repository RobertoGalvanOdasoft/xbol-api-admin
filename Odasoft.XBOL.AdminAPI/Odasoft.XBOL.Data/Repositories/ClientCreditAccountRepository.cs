using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Data.Repositories
{
    public class ClientCreditAccountRepository(XBOLDbContext dbContext) : BaseRepository<ClientCreditAccount>(dbContext)
    {
    }
}
