using Newtonsoft.Json;
using Odasoft.XBOL.Commons.Enums;

namespace XBOL.Admin.Core.DTO
{
    public class SeasonPassStatusRequest
    {
        public long SeasonPassId { get; set; }
        public SeasonPassStatus SeasonPassStatus { get; set; }

        [JsonProperty("status", Required = Required.AllowNull)]
        public SeasonPassSuspendedReason? SeasonPassSuspendedReason { get; set; } = null;
        public string? SuspendedOtherReason { get; set; } = null!;

    }
}
