using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Data.Repositories
{
    public class SuiteAgreementRepository(XBOLDbContext dbContext) : BaseRepository<SuiteAgreement>(dbContext)
    {
    }
}
