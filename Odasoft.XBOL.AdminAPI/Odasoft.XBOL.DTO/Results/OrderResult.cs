namespace Odasoft.XBOL.DTO.Results
{
    public class OrderResult
    {
        public long Id { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public string Event { get; set; } = "";
        public int NumberOfItems { get; set; }
        public decimal Amount { get; set; }
    }
}
