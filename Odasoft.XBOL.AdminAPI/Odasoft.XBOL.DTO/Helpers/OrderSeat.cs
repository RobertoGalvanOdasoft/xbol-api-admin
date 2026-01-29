namespace Odasoft.XBOL.DTO.Helpers
{
    public class OrderSeat
    {
        public long Id { get; init; }
        public decimal? PriceOverride { get; init; }
        public long BaseSeatId { get; init; }
        public long EventSectionId { get; init; }
        public string? DisplayName { get; init; }
        public string? SeatNumber { get; init; }
        public string? ExternalSeatObjectKey { get; init; }
    }
}
