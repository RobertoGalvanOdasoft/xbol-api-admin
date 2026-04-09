using Odasoft.XBOL.Commons.Enums;

namespace Odasoft.XBOL.DTO.Responses
{
    public class OrderRenewalInfoResponse
    {
        public long OrderId { get; set; }
        public bool IsSeasonOrder { get; set; }
        public List<OrderItemResponse> Items { get; set; } = new List<OrderItemResponse>();
        public string Event { get; set; } = "";

        public long? OrderSeasonId { get; set; }
        public long? CurrentSeasonId { get; set; }

        public long ClientId { get; set; }
        public string ClientName { get; set; } = "";
        public string DialCode { get; set; } = "";
        public long PhoneRegionCodeId { get; set; }
        public string PhoneNumber { get; set; } = "";
        public string Email { get; set; } = "";
        public string City { get; set; } = "";
        public string Neighbourhood { get; set; } = "";
        public Gender? Gender { get; set; }
        public DateTimeOffset? DOB { get; set; }
        public string Reference { get; set; } = "";

        public bool PendingRenewal
        {
            get
            {
                return CanRenew();
            }
        }

        private bool CanRenew()
        {
            if (this.IsSeasonOrder == false)
            {
                return false;
            }

            if (this.CurrentSeasonId == this.OrderSeasonId)
            {
                return false;
            }

            return this.Items.Any(i => !i.IsSold);
        }
    }
}
