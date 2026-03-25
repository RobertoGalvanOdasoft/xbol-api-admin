using Odasoft.XBOL.Commons.Enums;

namespace Odasoft.XBOL.DTO.Requests
{
    public class VenueRequest
    {
        public required string Name { get; set; } = "";
        public required VenueCategory Category { get; set; }
        public required VenueStatus Status { get; set; } = VenueStatus.Draft;
        public required string Country { get; set; } = "";
        public required string State { get; set; } = "";
        public required string City { get; set; } = "";
        public required string Neighborhood { get; set; } = "";
        public required string StreetAddress { get; set; } = "";
        public required string ExtNum { get; set; } = "";
        public string IntNum { get; set; } = "";
        public required string ZipCode { get; set; } = "";
        public required decimal Latitude { get; set; }
        public required decimal Longitude { get; set; }
        public string LandingUrl { get; set; } = "";
        public string ContactName { get; set; } = "";
        public string ContactEmail { get; set; } = "";
        public long? PhoneRegionCodeId { get; set; }
        public string DialCode { get; set; } = "";
        public string ContactPhoneNumber { get; set; } = "";
    }
}
