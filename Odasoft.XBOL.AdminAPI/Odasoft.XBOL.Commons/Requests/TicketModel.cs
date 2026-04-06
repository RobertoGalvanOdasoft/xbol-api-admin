namespace Odasoft.XBOL.Commons.Requests
{
    public class TicketModel
    {
        public string Event { get; set; } = "";
        public string OrderReference { get; set; } = "";
        public string QrBase64 { get; set; } = "";
        public string SeatKey { get; set; } = "";
    }
}
