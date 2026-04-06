namespace Odasoft.XBOL.DTO
{
    public class EventInfoDTO
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Subtitle { get; set; }
        public string? BannerImageUrl { get; set; }
        public List<Results.EventCategoryResult> Categories { get; set; } = [];
        public long? VenueMapId { get; set; }
        public string? VenueName { get; set; }
        public IEnumerable<SeatsIoPriceDTO> Prices { get; set; } = [];
        public IList<EventScheduleDTO> Schedules { get; set; } = [];
    }
}
