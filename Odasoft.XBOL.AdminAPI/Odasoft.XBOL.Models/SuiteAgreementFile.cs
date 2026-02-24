namespace Odasoft.XBOL.Models
{
    public class SuiteAgreementFile : BaseModel
    {
        public long SuiteAgreementId { get; set; }
        public SuiteAgreement SuiteAgreement { get; set; } = null!;
        public byte[] Content { get; set; } = [];
        public string ContentType { get; set; } = "";
        public string FileName { get; set; } = "";

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }
    }
}
