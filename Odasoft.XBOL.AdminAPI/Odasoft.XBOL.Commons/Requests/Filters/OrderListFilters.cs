using Odasoft.XBOL.Commons.Enums;

namespace Odasoft.XBOL.Commons.Requests.Filters
{
    public class OrderListFilters : BaseFilters
    {
        public long? SeasonId { get; set; }
        public bool RenovationMode { get; set; }
        public List<SeasonPassRenewalType>? RenewalTypes { get; set; } = [];
    }
}
