namespace Odasoft.XBOL.Models
{
    public class EventSeat : BaseModel
    {
        public long EventSectionId { get; set; }

        public EventSection EventSection { get; set; } = null!;

        public long BaseSeatId { get; set; }

        public decimal? PriceOverride { get; set; }

        public string ExternalSeatObjectKey { get; set; } = "";
    }
}
