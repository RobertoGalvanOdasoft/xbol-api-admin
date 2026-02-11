namespace Odasoft.XBOL.DTO
{
    public class VenueMapListItemDTO
    {
        public required long Id { get; set; }
        public required long VenueId { get; set; }
        public string? Name { get; set; }
        public string? ExternalMapKey { get; set; }
    }
}
