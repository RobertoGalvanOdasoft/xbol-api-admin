namespace Odasoft.XBOL.DTO
{
    public class EventInfoDTO
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string Subtitle { get; set; } = null!;
        public string BannerImageUrl { get; set; } = null!;
        public string Category { get; set; } = null!;
        public long? VenueMapId { get; set; }
        public string VenueName { get; set; } = null!;
        public IEnumerable<SeatsIoPriceDTO> Prices { get; set; } = [];
        public IList<EventScheduleDTO> Schedules { get; set; } = [];
    }
}
