using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Data.Repositories
{
    public class LegalRepresentativeRepository(XBOLDbContext dbContext) : BaseRepository<LegalRepresentative>(dbContext)
    {
    }
}
