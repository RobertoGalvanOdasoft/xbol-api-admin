using Odasoft.XBOL.Commons.Constants;

namespace Odasoft.XBOL.DTO.QueryParams
{
    public class BaseQueryParams
    {
        public string SearchTerm { get; set; } = "";
        public string SortBy { get; set; } = "";
        public required bool Descending { get; set; }
        public required int Page { get; set; } = QueryParamsConstants.DEFAULT_PAGE;
        public required int PageSize { get; set; } = QueryParamsConstants.DEFAULT_PAGE_SIZE;
    }
}
