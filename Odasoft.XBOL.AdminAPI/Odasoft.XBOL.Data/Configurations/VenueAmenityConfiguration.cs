using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Data.Configurations
{
    public class VenueAmenityConfiguration : IEntityTypeConfiguration<VenueAmenity>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<VenueAmenity> builder)
        {
            builder.HasKey(x => new { x.VenueId, x.AmenityId });
        }
    }
}
