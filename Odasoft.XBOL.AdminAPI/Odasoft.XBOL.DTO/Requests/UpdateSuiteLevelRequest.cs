namespace Odasoft.XBOL.DTO.Requests
{
    public class UpdateSuiteLevelRequest
    {
        public required long Id { get; set; }

        public required long VenueId { get; set; }

        public required string Name { get; set; } = "";
    }
}
