namespace Odasoft.XBOL.DTO
{
    public class EventAggregationDTO
    {
        public required long Id { get; set; }
        public required DateTimeOffset ScheduledStartDate { get; set; }
        public required string Name { get; set; }
        public List<Results.EventCategoryResult> Categories { get; set; } = [];
        public long? VenueMapId { get; set; }
        public string? VenueName { get; set; }
        public string? ExternalEventKey { get; set; }
        public required int AvailableSeats { get; set; }
        public required int TotalSeats { get; set; }
        public string? PosterImageUrl { get; set; }
        // TODO: Change this to an enum or a more robust type if needed to represent different types of events (e.g., regular event, season, etc.)
        public bool IsSeason { get; set; }
        public long? SeasonId { get; set; }
        public DateTimeOffset? OnSaleDate { get; set; }
        public DateTimeOffset? OffSaleDate { get; set; }
    }
}
