using Odasoft.XBOL.Commons.Enums;
using System.ComponentModel.DataAnnotations;

namespace Odasoft.XBOL.DTO.Requests
{
    public class CreateClientRequest
    {
        [Required]
        public string? CompanyName { get; set; }

        public string? SocialReason { get; set; }
        public string? RFC { get; set; }
        public ClientType? PersonTypeId { get; set; }
        public string? Country { get; set; }
        public string? State { get; set; }
        public string? Street { get; set; }
        public string? ExtNumber { get; set; }
        public string? IntNumber { get; set; }
        public string? PostalCode { get; set; }
        public string? Neighborhood { get; set; }
        public string? City { get; set; }
        public long? PhoneRegionCodeId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }

        public bool HasCredit { get; set; }

        public LegalRepForm LegalRep { get; set; } = new();
        public CreditForm Credit { get; set; } = new();
    }

    public class LegalRepForm
    {
        public string? Name { get; set; }
        public DateTimeOffset? Birthday { get; set; }
        public string? RFC { get; set; }
        public string? CURP { get; set; }
    }

    public class CreditForm
    {
        public decimal? AuthorizedAmount { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public PaymentFrequency? PaymentCycleTypeId { get; set; }
        public bool? InterestApply { get; set; }
        public decimal? TotalPlusInterest { get; set; }
        public int? TermInDays { get; set; }
    }
}
