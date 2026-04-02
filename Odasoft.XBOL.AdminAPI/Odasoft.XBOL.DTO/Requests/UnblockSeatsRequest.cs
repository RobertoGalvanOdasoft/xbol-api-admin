namespace Odasoft.XBOL.DTO.Requests;

public class UnblockSeatsRequest
{
    public IList<string>? SeatKeys { get; set; }
    public string? Reason { get; set; }
}
