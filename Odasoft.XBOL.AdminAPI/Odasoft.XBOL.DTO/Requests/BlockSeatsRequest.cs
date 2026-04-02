namespace Odasoft.XBOL.DTO.Requests;

public class BlockSeatsRequest
{
    public IList<string>? SeatKeys { get; set; }
    public string? Reason { get; set; }
    public string? Color { get; set; }
}
