namespace Odasoft.XBOL.Models
{
    public class Suite : BaseModel
    {
        public long SuiteLevelId { get; set; }
        public SuiteLevel SuiteLevel { get; set; } = null!;
        public string Name { get; set; } = "";
        public int Seats { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }
    }
}
