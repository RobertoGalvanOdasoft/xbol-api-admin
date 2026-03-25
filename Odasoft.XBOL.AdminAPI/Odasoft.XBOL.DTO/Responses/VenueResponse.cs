using Odasoft.XBOL.Commons.Enums;

namespace Odasoft.XBOL.DTO
{
    public class VenueResponse
    {
        public required long Id { get; set; }
        public string Name { get; set; } = "";
        public string Country { get; set; } = "";
        public string State { get; set; } = "";
        public string City { get; set; } = "";
        public string Neighborhood { get; set; } = "";
        public string StreetAddress { get; set; } = "";
        public string ExtNum { get; set; } = "";
        public string IntNum { get; set; } = "";
        public string ZipCode { get; set; } = "";
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string LandingUrl { get; set; } = "";
        public string ContactName { get; set; } = "";
        public string ContactEmail { get; set; } = "";
        public long? PhoneRegionCodeId { get; set; }
        public string DialCode { get; set; } = "";
        public string ContactPhoneNumber { get; set; } = "";

        public VenueCategory Category { get; set; }
        public VenueStatus Status { get; set; }
    }
}
