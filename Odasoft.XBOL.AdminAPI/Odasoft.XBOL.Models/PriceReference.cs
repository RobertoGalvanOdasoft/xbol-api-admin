
using Odasoft.XBOL.Commons.Enums;

namespace Odasoft.XBOL.Models
{
    public class PriceReference : BaseModel
    {
        public long PriceId { get; set; }
        public Price Price { get; set; } = null!;
        public SaleType ReferenceType { get; set; }
        public long ReferenceId { get; set; }
    }
}
