using Odasoft.XBOL.Commons.Enums;

namespace XBOL.Admin.Core.DTO
{
    public class SeatInfoRequest
    {
        public string SeatId { get; set; } = null!;
        public required decimal Price { get; set; }
        public SeatType CategoryEnum { get; set; }
        public string? Category { get; set; }
    }
}
