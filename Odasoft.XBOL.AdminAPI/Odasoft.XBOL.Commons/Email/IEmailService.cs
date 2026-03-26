namespace Odasoft.XBOL.Commons.Email;

public interface IEmailService
{
    Task SendAsync(
        string toAddress,
        string toName,
        string subject,
        string htmlBody,
        IEnumerable<EmailAttachment>? attachments = null,
        CancellationToken cancellationToken = default);
}
