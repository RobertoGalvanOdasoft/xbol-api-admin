namespace Odasoft.XBOL.Models
{
    public class PriceType : BaseModel
    {
        public string Name { get; set; } = null!;
        public string? Label { get; set; }
        public string? Description { get; set; }
        public bool Primary { get; set; }

        public IList<Price> Prices { get; set; } = [];
        public IList<PriceTypePrice> PriceTypePrices { get; set; } = [];
    }
}
