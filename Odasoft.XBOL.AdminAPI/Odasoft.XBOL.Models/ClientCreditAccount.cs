using Odasoft.XBOL.Commons.Enums;

namespace Odasoft.XBOL.Models
{
    public class ClientCreditAccount : BaseModel
    {
        public long ClientId { get; set; }
        public Client Client { get; set; } = null!;

        // Configuration
        public decimal CreditLimit { get; set; }

        public bool AppliesInterestRate { get; set; } // TODO: Pending interest rate logic
        public PaymentFrequency PaymentFrequency { get; set; }

        // Term
        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset? EndDate { get; set; }

        // Snapshot
        public decimal CurrentBalance { get; set; }

        public decimal AvailableAmount => CreditLimit - CurrentBalance;
        public CreditStatus CreditStatus { get; set; }
        public bool IsActive { get; set; }

        // Audit
        public DateTimeOffset CreatedAt { get; set; }

        public Guid CreatedBy { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public Guid UpdatedBy { get; set; }

        // The ledger / accounting book
        public IList<ClientCreditTransaction> ClientCreditTransactions { get; set; } = [];
    }
}
