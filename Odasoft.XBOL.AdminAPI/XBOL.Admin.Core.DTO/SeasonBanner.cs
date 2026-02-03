namespace XBOL.Admin.Core.DTO
{
    public class SeasonBanner
    {
        public string? ImageUrl { get; set; }
        public string? Title { get; set; }
        public string? Subtitle { get; set; }
        public string? Venue { get; set; }
        public DateTimeOffset? StartDateTime { get; set; }
        public DateTimeOffset? EndDateTime { get; set; }
    }
}
