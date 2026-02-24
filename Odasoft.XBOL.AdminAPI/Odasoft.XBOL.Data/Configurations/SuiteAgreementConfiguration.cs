using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Data.Configurations
{
    public class SuiteAgreementConfiguration : IEntityTypeConfiguration<SuiteAgreement>
    {
        public void Configure(EntityTypeBuilder<SuiteAgreement> builder)
        {
            builder.HasOne(sa => sa.Suite)
                   .WithMany(s => s.SuiteAgreements)
                   .HasForeignKey(sa => sa.SuiteId);

            builder.Property(sa => sa.OwnerName).HasMaxLength(200).IsRequired();
            builder.Property(sa => sa.OwnerEmail).HasMaxLength(200);
            builder.Property(sa => sa.OwnerPhone).HasMaxLength(50);
        }
    }
}
