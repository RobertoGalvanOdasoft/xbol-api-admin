using Odasoft.XBOL.Commons.Email;
using Odasoft.XBOL.Commons.Requests;

namespace Odasoft.XBOL.WorkerService.Jobs;

public partial class EmailJob(
    IEmailService emailService,
    ITemplateService templateService,
    ILogger<EmailJob> logger) : IEmailJob
{
    public async Task SendTestEmailAsync(TestEmailModel model)
    {
        try
        {
            LogProcessing(logger, model.ToAddress);

            var htmlBody = await templateService.RenderAsync("HelloWorld", model);
            await emailService.SendAsync(model.ToAddress, model.ToName, model.Subject, htmlBody);

            LogSent(logger, model.ToAddress);
        }
        catch (Exception ex)
        {
            LogFailed(logger, ex, model.ToAddress);
            throw;
        }
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Processing test email to {ToAddress}")]
    private static partial void LogProcessing(ILogger logger, string toAddress);

    [LoggerMessage(Level = LogLevel.Information, Message = "Test email sent successfully to {ToAddress}")]
    private static partial void LogSent(ILogger logger, string toAddress);

    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to send test email to {ToAddress}")]
    private static partial void LogFailed(ILogger logger, Exception ex, string toAddress);
}
