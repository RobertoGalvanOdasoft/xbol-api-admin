using Odasoft.XBOL.Commons.Enums;

namespace Odasoft.XBOL.Models
{
    public class Suite : BaseModel
    {
        public long SuiteLevelId { get; set; }
        public SuiteLevel SuiteLevel { get; set; } = null!;
        public string Name { get; set; } = "";
        public SuiteType SuiteType { get; set; }
        public int Capacity { get; set; }
        public string Policies { get; set; } = "";
        public string Amenities { get; set; } = "";
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }

        public IList<SuiteAgreement> SuiteAgreements { get; set; } = [];
    }
}
