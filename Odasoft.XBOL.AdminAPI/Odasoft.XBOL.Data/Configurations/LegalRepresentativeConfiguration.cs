using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Data.Configurations
{
    public class LegalRepresentativeConfiguration : IEntityTypeConfiguration<LegalRepresentative>
    {
        public void Configure(EntityTypeBuilder<LegalRepresentative> builder)
        {
            builder.Property(x => x.ClientId).IsRequired();

            builder.HasOne(x => x.Client)
                .WithOne(x => x.LegalRepresentative);
        }
    }
}
