using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Data.Configurations
{
    public class VenueConfiguration : IEntityTypeConfiguration<Venue>
    {
        public void Configure(EntityTypeBuilder<Venue> builder)
        {
            builder.Property(x => x.ContactPhoneNumber).HasMaxLength(15);
            builder.Property(x => x.ZipCode).HasMaxLength(10);

            builder.Property(x => x.Latitude).HasPrecision(11, 8);
            builder.Property(x => x.Longitude).HasPrecision(11, 8);

            builder.HasMany(x => x.VenueMaps)
                .WithOne(x => x.Venue)
                .HasForeignKey(x => x.VenueId);

            builder.HasMany(x => x.VenueImages)
                .WithOne(x => x.Venue)
                .HasForeignKey(x => x.VenueId);
        }
    }
}
