namespace Odasoft.XBOL.Models
{
    public class PriceTypePrice
    {
        public long PriceId { get; set; }
        public long PriceTypeId { get; set; }
        public decimal TypePrice { get; set; }

        public Price Price { get; set; } = null!;
        public PriceType PriceType { get; set; } = null!;
    }
}
