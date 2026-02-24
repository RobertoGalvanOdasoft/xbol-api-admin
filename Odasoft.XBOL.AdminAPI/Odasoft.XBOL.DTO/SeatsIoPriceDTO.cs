namespace Odasoft.XBOL.DTO
{
    public class SeatsIoPriceDTO
    {
        public decimal Price { get; set; }
        public string[]? Objects { get; set; }
        public long? Category { get; set; }
        public decimal? OriginalPrice { get; set; }
        public decimal? Fee { get; set; }
        public TicketTypePriceDTO[]? TicketTypes { get; set; }
    }
}
