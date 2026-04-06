namespace Odasoft.XBOL.Commons.Requests;

// TODO: Move this to a proper namespace like Odasoft.XBOL.Commons.Email.Models
public class EmailModelBase
{
    public required string ToAddress { get; set; }
    public required string ToName { get; set; }
    public required string Subject { get; set; }
    public string Culture { get; set; } = "es-MX";
}
