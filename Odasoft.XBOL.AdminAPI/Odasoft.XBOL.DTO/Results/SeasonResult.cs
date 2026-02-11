namespace Odasoft.XBOL.DTO.Results
{
    public class SeasonResult
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string BannerImageUrl { get; set; } = null!;
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public string ExternalSeasonKey { get; set; } = null!;
        public string? Venue { get; set; }
        public IEnumerable<SeatsIoPriceDTO> Prices { get; set; } = [];
    }
}
