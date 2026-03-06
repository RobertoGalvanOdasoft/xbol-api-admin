namespace Odasoft.XBOL.Commons.Requests.Filters
{
    public class ClientFilter
    {
        public long? PhoneRegionCodeId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public required long SeasonId { get; set; }
    }
}
