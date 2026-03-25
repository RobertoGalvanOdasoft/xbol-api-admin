using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Data.Configurations
{
    public class SuiteConfiguration : IEntityTypeConfiguration<Suite>
    {
        public void Configure(EntityTypeBuilder<Suite> builder)
        {
            builder.Property(s => s.Policies).HasColumnType("text");
            builder.Property(s => s.Amenities).HasColumnType("text");

            builder.HasMany(s => s.SuiteAgreements)
                   .WithOne(sa => sa.Suite)
                   .HasForeignKey(sa => sa.SuiteId);
        }
    }
}
