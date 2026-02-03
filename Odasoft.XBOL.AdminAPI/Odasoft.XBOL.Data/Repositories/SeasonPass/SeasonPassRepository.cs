using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Commons.Enums;
using XBOL.Admin.Core.DTO;

namespace Odasoft.XBOL.Data.Repositories.Season
{
    public class SeasonPassRepository(XBOLDbContext dbContext) : BaseRepository<Models.SeasonPass>(dbContext)
    {

    }
}
