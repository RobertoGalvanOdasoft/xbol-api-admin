using Odasoft.XBOL.Commons.Enums;

namespace Odasoft.XBOL.DTO.Results
{
    public class SeasonResult
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string? Description { get; set; }
        public string? BannerImageUrl { get; set; }
        public string? PosterImageUrl { get; set; }
        public string? LandingUrl { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public DateTimeOffset? PublishedDate { get; set; }
        public DateTimeOffset? OnSaleDate { get; set; }
        public DateTimeOffset? PreSaleDate { get; set; }
        public DateTimeOffset? OffSaleDate { get; set; }
        public SeasonStatus Status { get; set; }
        public string ExternalSeasonKey { get; set; } = null!;
        public string? Venue { get; set; }
        public IEnumerable<SeatsIoPriceDTO> Prices { get; set; } = [];
    }
}
