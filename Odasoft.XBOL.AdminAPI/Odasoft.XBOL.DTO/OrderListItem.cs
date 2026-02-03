using Odasoft.XBOL.Commons.Enums;

namespace XBOL.Admin.Core.DTO
{
    public class OrderListItem
    {
        public long OrderId { get; set; }
        public string Order { get; set; } = null!;
        public string CountryPhoneISO { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public required int SeatCount { get; set; }
        public required decimal Total { get; set; }
        public SeasonPeriod SeasonPeriod { get; set; }
        public string? RelatedOrderReference { get; set; }
        public IList<SeasonPassItem> SeasonPasses { get; set; } = [];
    }
}
