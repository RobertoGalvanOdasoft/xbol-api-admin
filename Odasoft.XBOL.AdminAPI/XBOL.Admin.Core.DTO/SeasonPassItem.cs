using Odasoft.XBOL.Commons.Enums;
using Newtonsoft.Json;

namespace XBOL.Admin.Core.DTO
{
    public class SeasonPassItem
    {
        public long SeasonPassId { get; set; }
        public string SeatCode { get; set; } = null!;
        public decimal Price { get; set; }
        public SeatType CategoryEnum { get; set; }
        public string? Category { get; set; }

        [JsonProperty("status", Required = Required.AllowNull)]
        public SeasonPassStatus? Status { get; set; } = null;
        [JsonProperty("suspendedReason", Required = Required.AllowNull)]
        public SeasonPassSuspendedReason? SuspendedReason { get; set; } = null;
        public string? SuspendedOtherReason { get; set; }
    }
}
