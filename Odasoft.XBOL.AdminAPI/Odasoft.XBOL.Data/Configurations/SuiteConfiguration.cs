using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Data.Configurations
{
    public class SuiteConfiguration : IEntityTypeConfiguration<Suite>
    {
        public void Configure(EntityTypeBuilder<Suite> builder)
        {
            builder.HasMany(s => s.SuiteAgreements)
                   .WithOne(sa => sa.Suite)
                   .HasForeignKey(sa => sa.SuiteId);
        }
    }
}
