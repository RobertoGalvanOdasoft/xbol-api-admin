namespace Odasoft.XBOL.DTO.Responses
{
    public class OrderItemResponse
    {
        public long Id { get; set; }
        public bool IsSeasonItem { get; set; }
        public string SeatObjectKey { get; set; } = "";
        public string Zone { get; set; } = "";
        public string Section { get; set; } = "";
        public string Row { get; set; } = "";
        public string Seat { get; set; } = "";
        public bool IsSold { get; set; }
        public decimal Price { get; set; }
        public decimal RenewalPrice { get; set; }
    }
}
