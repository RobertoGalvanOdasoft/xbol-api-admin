namespace Odasoft.XBOL.DTO.Results
{
    public class SeatPricesResult
    {
        public string Message { get; set; } = "";

        public IList<SeatPriceDTO> SeatPrices { get; set; } = [];
    }
}
