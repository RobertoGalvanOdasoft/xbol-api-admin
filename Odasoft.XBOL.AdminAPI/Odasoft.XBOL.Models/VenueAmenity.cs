namespace Odasoft.XBOL.Models
{
    public class VenueAmenity
    {
        public long VenueId { get; set; }
        public Venue Venue { get; set; } = null!;
        public long AmenityId { get; set; }
        public Amenity Amenity { get; set; } = null!;
    }
}
