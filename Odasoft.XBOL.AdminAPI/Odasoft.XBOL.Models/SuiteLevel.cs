namespace Odasoft.XBOL.Models
{
    public class SuiteLevel : BaseModel
    {
        public long VenueId { get; set; }
        public Venue Venue { get; set; } = null!;
        public string Name { get; set; } = "";
        public IList<Suite> Suites { get; set; } = [];
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }
    }
}
