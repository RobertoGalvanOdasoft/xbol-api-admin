namespace Odasoft.XBOL.DTO.Responses
{
    public class EventImageResponse
    {
        public long Id { get; set; }
        public long EventId { get; set; }
        public string Title { get; set; } = "";
        public string ImageBase64 { get; set; } = "";
        public string ContentType { get; set; } = "";
    }
}
