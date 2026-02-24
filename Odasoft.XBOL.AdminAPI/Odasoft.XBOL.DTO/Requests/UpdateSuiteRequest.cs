namespace Odasoft.XBOL.DTO.Requests
{
    public class UpdateSuiteRequest
    {
        public required long Id { get; set; }

        public required string Name { get; set; } = "";

        public required long SuiteLevelId { get; set; }

        public required int Seats { get; set; }
    }
}
