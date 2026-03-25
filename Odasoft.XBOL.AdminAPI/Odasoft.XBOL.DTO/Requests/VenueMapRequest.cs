namespace Odasoft.XBOL.DTO.Requests
{
    public class VenueMapRequest
    {
        public long? Id { get; set; }
        public long VenueId { get; set; }
        public string Name { get; set; } = "";
        public string ExternalMapKey { get; set; } = "";
        public int Capacity { get; set; }
    }
}
