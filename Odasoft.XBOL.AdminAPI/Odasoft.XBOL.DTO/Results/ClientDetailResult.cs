using Odasoft.XBOL.Commons.Enums;

namespace Odasoft.XBOL.DTO.Results
{
    public class ClientDetailResult
    {
        public required long Id { get; set; }
        public ClientType ClientType { get; set; }
        public string? ClientName { get; set; }
        public string? BusinessName { get; set; }
        public Gender? Gender { get; set; }
        public DateTimeOffset? DateOfBirth { get; set; }
        public string? Email { get; set; }
        public long? PhoneRegionCodeId { get; set; }
        public string? DialCode { get; set; }
        public string? PhoneNumber { get; set; }
        public string? TaxId { get; set; }
        public string? Country { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string? StreetAddress { get; set; }
        public string? ExtNum { get; set; }
        public string? IntNum { get; set; }
        public string? PostalCode { get; set; }
        public string? Neighborhood { get; set; }
        public LegalRepresentativeDTO? LegalRepresentative { get; set; }
        public ClientCreditDTO? ClientCredit { get; set; }
    }

    public class LegalRepresentativeDTO
    {
        public required long Id { get; set; }
        public required long ClientId { get; set; }
        public string FullName { get; set; } = "";
        public DateTimeOffset? DOB { get; set; }
        public string? TaxId { get; set; }
        public string? CURP { get; set; }
    }

    public class ClientCreditDTO
    {
        public required long Id { get; set; }
        public required long ClientId { get; set; }
        public decimal? CreditLimit { get; set; }
        public bool AppliesInterestRate { get; set; }
        public PaymentFrequency? PaymentFrequency { get; set; }
        public string? PaymentFrequencyLabel { get; set; }
        public decimal? PendingAmount { get; set; }
        public decimal? AmountPaid { get; set; }
        public CreditStatus? CreditStatus { get; set; }
        public string? CreditStatusLabel { get; set; }
        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset? EndDate { get; set; }
    }
}
