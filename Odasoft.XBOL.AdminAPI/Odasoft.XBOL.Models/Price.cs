namespace Odasoft.XBOL.Models
{
    public class Price : BaseModel
    {
        public long BaseZoneId { get; set; }
        public BaseZone BaseZone { get; set; }
        public decimal? ZonePrice { get; set; }
        public long? BaseSectionId { get; set; }
        public BaseSection? BaseSection { get; set; }
        public decimal? SectionPrice { get; set; }
        public long? BaseRowId { get; set; }
        public BaseRow? BaseRow { get; set; }
        public decimal? RowPrice { get; set; }
        public long? BaseSeatId { get; set; }
        public BaseSeat? BaseSeat { get; set; }
        public decimal? SeatPrice { get; set; }


        public long VenueMapId { get; set; }
        public VenueMap VenueMap { get; set; } = null!;
        public IList<PriceType> PriceTypes { get; set; } = [];
        public IList<PriceTypePrice> PriceTypePrices { get; set; } = [];
    }
}
