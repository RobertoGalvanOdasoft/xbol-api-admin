namespace Odasoft.XBOL.DTO.Results
{
    public class SuiteResult
    {
        public required long Id { get; set; }

        public required long SuiteLevelId { get; set; }
        public string SuiteLevelName { get; set; } = "";

        public string Name { get; set; } = "";

        public required int Seats { get; set; }
    }
}
