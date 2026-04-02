namespace Odasoft.XBOL.Models
{
    public class EventSeat : BaseModel
    {
        public long EventSectionId { get; set; }

        public EventSection EventSection { get; set; } = null!;

        public long BaseSeatId { get; set; }
        public BaseSeat BaseSeat { get; set; } = null!;

        public decimal? PriceOverride { get; set; }

        public IList<PriceRule> PriceRules { get; set; } = [];
        public IList<Ticket> Tickets { get; set; } = [];
        public bool ForSale { get; set; } = true;
        public string ExternalSeatObjectKey { get; set; } = "";
    }
}
