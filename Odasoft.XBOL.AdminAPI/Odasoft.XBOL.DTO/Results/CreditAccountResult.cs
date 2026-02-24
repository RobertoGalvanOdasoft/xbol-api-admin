using Odasoft.XBOL.Commons.Enums;

namespace Odasoft.XBOL.DTO.Results
{
    public class CreditAccountResult
    {
        public required long Id { get; set; }
        public required long ClientId { get; set; }
        public required decimal CreditLimit { get; set; }
        public required DateTimeOffset StartDate { get; set; }
        public required PaymentFrequency PaymentFrequency { get; set; }
        public required decimal AmountPaid { get; set; }
        public required decimal PendingAmount { get; set; }
        public CreditStatus CreditStatus { get; set; }
    }
}
