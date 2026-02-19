using Odasoft.XBOL.Commons.Enums;

namespace Odasoft.XBOL.DTO.Requests
{
    public class CreateClientCreditAccountRequest
    {
        public required long ClientId { get; set; }
        public required decimal CreditLimit { get; set; }
        public bool AppliesInterestRate { get; set; }
        public PaymentFrequency PaymentFrequency { get; set; }
        public required DateTimeOffset StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
    }
}
