using Odasoft.XBOL.Commons.Enums;

namespace Odasoft.XBOL.DTO.Results
{
    public class ClientResult
    {
        public required long Id { get; set; }
        public required string ClientName { get; set; }
        public string? LegalRepName { get; set; }
        public required bool HasCredit { get; set; }
        public decimal? CreditAmount { get; set; }
        public decimal? PendingAmount { get; set; }
        public decimal? AmountPaid { get; set; }
        public CreditStatus? CreditStatus { get; set; }
    }
}
