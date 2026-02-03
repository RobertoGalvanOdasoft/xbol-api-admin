namespace Odasoft.XBOL.DTO
{
    public class ZonePriceDTO
    {
        public long Category { get; set; }
        public TicketTypePriceDTO[]? TicketTypes { get; set; }
        public decimal? Price { get; set; }
    }
}
