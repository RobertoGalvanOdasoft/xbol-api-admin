namespace Odasoft.XBOL.DTO.Helpers
{
    public class OrderSeat
    {
        public required long Id { get; init; }
        public required decimal? PriceOverride { get; init; }
        public required long BaseSeatId { get; init; }
        public required long EventSectionId { get; init; }
        public string? DisplayName { get; init; }
        public string? SeatNumber { get; init; }
        public string? ExternalSeatObjectKey { get; init; }
    }
}
