namespace Odasoft.XBOL.DTO.Requests
{
    public interface ISuiteRequest
    {
        string Name { get; set; }
        long SuiteLevelId { get; set; }
        int Seats { get; set; }
    }
}
