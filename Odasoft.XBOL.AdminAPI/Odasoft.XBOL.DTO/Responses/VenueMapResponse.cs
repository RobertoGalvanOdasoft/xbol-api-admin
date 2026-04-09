namespace Odasoft.XBOL.DTO
{
    public class VenueMapResponse
    {
        public required long Id { get; set; }
        public required long VenueId { get; set; }
        public string? Name { get; set; }
        public string? ExternalMapKey { get; set; }
        public int Capacity { get; set; }
        public string? ThumbnailUrl { get; set; }
    }
}
