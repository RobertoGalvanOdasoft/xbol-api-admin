using Odasoft.XBOL.Commons.Enums;

namespace Odasoft.XBOL.DTO.Requests
{
    public interface ISuiteRequest
    {
        string Name { get; set; }
        long SuiteLevelId { get; set; }
        SuiteType SuiteType { get; set; }
        int Capacity { get; set; }
        string Policies { get; set; }
        string Amenities { get; set; }
    }
}
