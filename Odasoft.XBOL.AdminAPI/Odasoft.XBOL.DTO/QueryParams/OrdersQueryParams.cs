namespace Odasoft.XBOL.DTO.QueryParams
{
    public class OrdersQueryParams : BaseQueryParams
    {
        public long? ClientId { get; set; }
        public List<string> Events { get; set; } = [];
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
    }
}
