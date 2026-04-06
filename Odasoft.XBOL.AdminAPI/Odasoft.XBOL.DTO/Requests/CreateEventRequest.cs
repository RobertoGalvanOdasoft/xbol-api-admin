namespace Odasoft.XBOL.DTO.Requests
{
    public class CreateEventRequest : IEventRequest
    {
        public long? VenueMapId { get; set; }
        public string Name { get; set; } = null!;
        public string? Subtitle { get; set; }
        public string? ShortDescription { get; set; }
        public string? LongDescription { get; set; }

        public List<long> CategoryIds { get; set; } = [];
    }
}
