using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Data.Configurations
{
    public class SuiteAgreementFileConfiguration : IEntityTypeConfiguration<SuiteAgreementFile>
    {
        public void Configure(EntityTypeBuilder<SuiteAgreementFile> builder)
        {
            builder.HasOne(f => f.SuiteAgreement)
               .WithOne(sa => sa.SuiteAgreementFile)
               .HasForeignKey<SuiteAgreementFile>(f => f.SuiteAgreementId);

            builder.Property(f => f.FileName).HasMaxLength(255).IsRequired();
            builder.Property(f => f.ContentType).HasMaxLength(100).IsRequired();
            builder.Property(f => f.Content).HasColumnType("bytea");
        }
    }
}
