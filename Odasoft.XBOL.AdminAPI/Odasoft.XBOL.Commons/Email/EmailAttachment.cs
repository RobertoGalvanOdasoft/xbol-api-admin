namespace Odasoft.XBOL.Commons.Email;

public class EmailAttachment
{
    public required string ContentId { get; set; }
    public required byte[] Content { get; set; }
    public required string ContentType { get; set; }
    public string? FileName { get; set; }
    public bool IsInline { get; set; } = true;
}
