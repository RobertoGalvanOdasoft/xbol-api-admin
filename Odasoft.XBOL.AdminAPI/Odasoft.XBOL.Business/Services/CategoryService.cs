using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Data;
using Odasoft.XBOL.DTO.Results;

namespace Odasoft.XBOL.Business.Services
{
    public class CategoryService(XBOLDbContext dbContext)
    {
        public async Task<List<EventCategoryResult>> GetCategoriesAsync()
        {
            return await dbContext.EventCategories
                .AsNoTracking()
                .Where(c => c.IsActive)
                .Select(c => new EventCategoryResult
                {
                    Id = c.Id,
                    Name = c.Name,
                    DisplayName = c.DisplayName,
                    IsActive = c.IsActive,
                })
                .ToListAsync();
        }
    }
}
