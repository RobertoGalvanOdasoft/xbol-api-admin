using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.DTO.Response;

namespace Odasoft.XBOL.Business.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<PagedResponse<T>> ToPagedResponseAsync<T>(this IQueryable<T> query, int page, int pageSize)
        {
            var totalCount = await query.CountAsync();

            var items = await query
                                .Skip(page * pageSize)
                                .Take(pageSize)
                                .ToListAsync();

            return new PagedResponse<T>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
