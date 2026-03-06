namespace Odasoft.XBOL.Models
{
    public class SuiteAgreement : BaseModel
    {
        public long SuiteId { get; set; }
        public Suite Suite { get; set; } = null!;
        public string OwnerName { get; set; } = "";
        public string Email { get; set; } = "";
        public long? PhoneRegionCodeId { get; set; }
        public PhoneRegionCode? PhoneRegionCode { get; set; }
        public string PhoneNumber { get; set; } = "";

        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }

        public SuiteAgreementFile? SuiteAgreementFile { get; set; } = null!;
    }
}
