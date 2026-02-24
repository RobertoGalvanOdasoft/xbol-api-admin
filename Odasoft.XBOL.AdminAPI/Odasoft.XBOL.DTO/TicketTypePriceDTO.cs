namespace Odasoft.XBOL.DTO
{
    public class TicketTypePriceDTO
    {
        public string TicketType { get; set; } = "";
        public decimal Price { get; set; }
        public string? Label { get; set; }
        public string? Description { get; set; }
        public bool? Primary { get; set; }
    }
}
