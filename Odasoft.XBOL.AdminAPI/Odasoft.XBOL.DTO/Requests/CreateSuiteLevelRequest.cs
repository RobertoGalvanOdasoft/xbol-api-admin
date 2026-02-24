namespace Odasoft.XBOL.DTO.Requests
{
    public class CreateSuiteLevelRequest
    {
        public required long VenueId { get; set; }

        public required string Name { get; set; } = "";
    }
}
