using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Data.Configurations
{
    public class ClientCreditTransactionConfiguration : IEntityTypeConfiguration<ClientCreditTransaction>
    {
        public void Configure(EntityTypeBuilder<ClientCreditTransaction> builder)
        {
            builder.Property(x => x.ClientCreditAccountId).IsRequired();

            builder.Property(x => x.Amount)
                .HasColumnType("decimal(19,4)");
        }
    }
}
