namespace Odasoft.XBOL.DTO.Results
{
    public class LegalRepresentativeResult
    {
        public required long Id { get; set; }
        public required long ClientId { get; set; }
        public string FullName { get; set; } = "";
        public DateTimeOffset DOB { get; set; }
        public string TaxId { get; set; } = "";
        public string CURP { get; set; } = "";
    }
}
