using Odasoft.XBOL.Commons.Enums;

namespace Odasoft.XBOL.DTO.Requests
{
    public class UpdateEventRequest : IEventRequest
    {
        public long Id { get; set; }
        public long? VenueMapId { get; set; }
        public string Name { get; set; } = null!;
        public string? Subtitle { get; set; }
        public string? ShortDescription { get; set; }
        public string? LongDescription { get; set; }

        public EventCategory? Category { get; set; }
        public EventStatus Status { get; set; }
    }
}
