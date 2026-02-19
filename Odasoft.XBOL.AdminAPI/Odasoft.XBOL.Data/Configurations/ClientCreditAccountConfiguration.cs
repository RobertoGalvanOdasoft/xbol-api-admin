using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Data.Configurations
{
    public class ClientCreditAccountConfiguration : IEntityTypeConfiguration<ClientCreditAccount>
    {
        public void Configure(EntityTypeBuilder<ClientCreditAccount> builder)
        {
            builder.Property(x => x.ClientId).IsRequired();

            builder.Property(x => x.AvailableAmount)
                .HasColumnType("decimal(19,4)");

            builder.Property(x => x.CreditLimit)
                .HasColumnType("decimal(19,4)");

            builder.HasMany(x => x.ClientCreditTransactions)
                .WithOne(x => x.ClientCreditAccount);

            builder.HasOne(x => x.Client)
                .WithOne(x => x.ClientCreditAccount);

            // Ignore as it is calculated
            builder.Ignore(x => x.AvailableAmount);
        }
    }
}
