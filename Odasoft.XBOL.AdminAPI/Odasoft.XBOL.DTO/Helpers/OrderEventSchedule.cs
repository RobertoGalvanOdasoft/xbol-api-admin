namespace Odasoft.XBOL.DTO.Helpers
{
    public class OrderEventSchedule
    {
        public required long Id { get; init; }
        public List<OrderSeat> Seats { get; init; } = [];
    }
}
