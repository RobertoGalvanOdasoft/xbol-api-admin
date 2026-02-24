namespace Odasoft.XBOL.DTO
{
    public class SeatPriceDTO
    {
        public required decimal Price { get; set; }
        public string SeatKey { get; set; } = "";
    }
}
