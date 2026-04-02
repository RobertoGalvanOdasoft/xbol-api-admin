namespace Odasoft.XBOL.DTO.Results
{
    public class BookingResult
    {
        public long? BookingId { get; set; }
        public string? Reference { get; set; }
        public string Message { get; set; } = "";
        public IEnumerable<string> Tickets { get; set; } = [];
        public string? ClientPhone { get; set; }
        public string? ClientEmail { get; set; }
        public required string Localizer { get; set; }
    }
}
