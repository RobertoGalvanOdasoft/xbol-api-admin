using Odasoft.XBOL.Commons.Enums;

namespace Odasoft.XBOL.DTO.Responses
{
    public class OrderInfoResponse
    {
        public long OrderId { get; set; }
        public OrderType Type { get; set; }
        public PayformType Channel { get; set; }
        public OrderStatus Status { get; set; }
        public string Reference { get; set; } = "";
        public DateTimeOffset OrderDateTime { get; set; }
        public int ItemQuantity { get; set; }
        public string Seller { get; set; } = "";
        public decimal Total { get; set; }
        public PaymentType PaymentMethod { get; set; }

        public List<OrderItemResponse> Items { get; set; } = new List<OrderItemResponse>();

        public string Event { get; set; } = "";
        public long ClientId { get; set; }
        public string ClientName { get; set; } = "";
        public string DialCode { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string Email { get; set; } = "";
    }
}
