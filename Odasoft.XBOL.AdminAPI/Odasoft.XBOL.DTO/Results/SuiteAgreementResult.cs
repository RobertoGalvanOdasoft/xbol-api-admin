namespace Odasoft.XBOL.DTO.Results
{
    public class SuiteAgreementResult
    {
        public required long Id { get; set; }
        public required long SuiteId { get; set; }
        public string SuiteName { get; set; } = "";
        public required long SuiteLevelId { get; set; }
        public string SuiteLevel { get; set; } = "";
        public string OwnerName { get; set; } = "";
        public string Email { get; set; } = "";
        public long? PhoneRegionCodeId { get; set; }
        public string DialCode { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public required DateTimeOffset StartDate { get; set; }
        public required DateTimeOffset EndDate { get; set; }
        public string FileName { get; set; } = "";
    }
}
