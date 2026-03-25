namespace Odasoft.XBOL.DTO.Responses
{
    public class VenueImageResponse
    {
        public long Id { get; set; }
        public long VenueId { get; set; }
        public string Title { get; set; } = "";
        public string ImageBase64 { get; set; } = "";
        public string ContentType { get; set; } = "";
    }
}
