using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Data.Repositories
{
    public class RoleRepository(XBOLDbContext dbContext)
    {
        public async Task<Role> GetByIdAsync(Guid id) => await dbContext
                                                                .Roles
                                                                .FirstOrDefaultAsync(x => x.Id == id)
                                                                ?? throw new Exception($"{nameof(Role)} Not Found");
    }
}
