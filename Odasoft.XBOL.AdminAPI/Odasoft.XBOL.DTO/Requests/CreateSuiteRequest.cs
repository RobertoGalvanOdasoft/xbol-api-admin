namespace Odasoft.XBOL.DTO.Requests
{
    public class CreateSuiteRequest : ISuiteRequest
    {
        public required string Name { get; set; } = "";

        public required long SuiteLevelId { get; set; }

        public required int Seats { get; set; }
    }
}
