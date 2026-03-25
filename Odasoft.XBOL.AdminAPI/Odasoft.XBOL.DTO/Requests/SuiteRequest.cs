using Odasoft.XBOL.Commons.Enums;

namespace Odasoft.XBOL.DTO.Requests
{
    public class SuiteRequest : ISuiteRequest
    {
        public required string Name { get; set; } = "";

        public required long SuiteLevelId { get; set; }

        public required SuiteType SuiteType { get; set; }

        public required int Capacity { get; set; }
        public string Policies { get; set; } = "";
        public string Amenities { get; set; } = "";
    }
}
