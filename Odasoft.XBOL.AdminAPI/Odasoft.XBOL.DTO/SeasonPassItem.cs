using Odasoft.XBOL.Commons.Enums;

namespace XBOL.Admin.Core.DTO
{
    public class SeasonPassItem
    {
        public required long SeasonPassId { get; set; }
        public string SeatCode { get; set; } = null!;
        public required decimal Price { get; set; }
        public SeatType CategoryEnum { get; set; }
        public string? Category { get; set; }
        public SeasonPassStatus? Status { get; set; }
        public SeasonPassSuspendedReason? SuspendedReason { get; set; }
        public string? SuspendedOtherReason { get; set; }
    }
}
