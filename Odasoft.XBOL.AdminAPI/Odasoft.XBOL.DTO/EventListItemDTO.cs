namespace Odasoft.XBOL.DTO
{
    public class EventListItemDTO
    {
        public required long Id { get; set; }
        public required DateTimeOffset ScheduledStartDate { get; set; }
        public required string Name { get; set; }
        public string? Category { get; set; }
        public required long VenueMapId { get; set; }
        public string? VenueName { get; set; }
        public string? ExternalEventKey { get; set; }
        public required int AvailableSeats { get; set; }
        public required int TotalSeats { get; set; }
    }
}
