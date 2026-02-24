namespace Odasoft.XBOL.Commons.Requests.Filters
{
    public class ClientFilter
    {
        public string? CountryPhoneISO { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public required long SeasonId { get; set; }
    }
}
