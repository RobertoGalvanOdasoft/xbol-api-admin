using Odasoft.XBOL.Commons.Enums;

namespace XBOL.Admin.Core.DTO
{
    public class SeasonPassStatusRequest
    {
        public required long SeasonPassId { get; set; }
        public SeasonPassStatus SeasonPassStatus { get; set; }
        public SeasonPassSuspendedReason? SeasonPassSuspendedReason { get; set; }
        public string? SuspendedOtherReason { get; set; } = null!;
    }
}
