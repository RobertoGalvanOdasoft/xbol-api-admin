namespace XBOL.Admin.Core.DTO
{
    public class ClientSeasonEvent
    {
        public ClientContactRequest? ClientContact { get; set; }
        public string EventKey { get; set; } = string.Empty;
        public long EventId { get; set; }
        public bool AlreadyRenewed { get; set; }
        public bool CanRenovate { get; set; }
        public long? ClientId { get; set; }
        public IList<SeatInfoRequest> Seats { get; set; } = [];
        public long SeasonId { get; set; }
        public string SeasonKey { get; set; } = string.Empty;
        public long? RelatedOrderId { get; set; }
    }
}
