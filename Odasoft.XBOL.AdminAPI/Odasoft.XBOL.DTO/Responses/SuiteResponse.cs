using Odasoft.XBOL.Commons.Enums;

namespace Odasoft.XBOL.DTO.Responses
{
    public class SuiteResponse
    {
        public required long Id { get; set; }
        public required long VenueId { get; set; }
        public string VenueName { get; set; } = "";
        public required long SuiteLevelId { get; set; }
        public string SuiteLevelName { get; set; } = "";
        public string Name { get; set; } = "";
        public SuiteType SuiteType { get; set; }
        public required int Capacity { get; set; }
        public string Policies { get; set; } = "";
        public string Amenities { get; set; } = "";
    }
}
