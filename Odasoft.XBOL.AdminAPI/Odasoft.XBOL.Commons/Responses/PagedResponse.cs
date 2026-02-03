namespace Odasoft.XBOL.Commons.Responses
{
    // TODO: Use XBOL.DTO

    public class PagedResponse<T>
    {
        public IReadOnlyList<T> Items { get; set; } = [];

        public required int CurrentPage { get; set; }
        public required int PageSize { get; set; }

        public required int TotalItems { get; set; }
        public required int TotalPages { get; set; }
    }
}
