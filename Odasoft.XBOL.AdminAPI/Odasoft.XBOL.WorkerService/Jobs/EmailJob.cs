using Microsoft.Extensions.Localization;
using Odasoft.XBOL.Commons.Email;
using Odasoft.XBOL.Commons.Requests;
using System.Globalization;

namespace Odasoft.XBOL.WorkerService.Jobs;

public partial class EmailJob(
    IEmailService emailService,
    ITemplateService templateService,
    IStringLocalizer<EmailResource> localizer,
    ILogger<EmailJob> logger) : IEmailJob
{
    private static readonly string BasePath = AppContext.BaseDirectory;

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

    public async Task SendOrderConfirmationAsync(OrderConfirmationModel model)
    {
        try
        {
            LogProcessingOrderConfirmation(logger, model.ToAddress, model.OrderDetails.OrderNumber);

            var culture = new CultureInfo(model.Culture);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            var attachments = new List<EmailAttachment>();

            // Attach Logo
            var logoPath = model.Theme.LogoPath;
            if (!string.IsNullOrEmpty(logoPath) && File.Exists(Path.Combine(BasePath, logoPath)))
            {
                AddLocalAttachment(attachments, "logo", logoPath);
            }

            // Attach Wallet Badges (locale-specific)
            AddLocalAttachment(attachments, "google-wallet", localizer["Wallet_GoogleBadgePath"].Value);
            AddLocalAttachment(attachments, "apple-wallet", localizer["Wallet_AppleBadgePath"].Value);

            var htmlBody = await templateService.RenderAsync("OrderConfirmation", model);
            await emailService.SendAsync(model.ToAddress, model.ToName, model.Subject, htmlBody, attachments);

            LogOrderConfirmationSent(logger, model.ToAddress, model.OrderDetails.OrderNumber);
        }
        catch (Exception ex)
        {
            LogOrderConfirmationFailed(logger, ex, model.ToAddress, model.OrderDetails.OrderNumber);
            throw;
        }
    }

    private bool AddLocalAttachment(List<EmailAttachment> attachments, string contentId, string relativePath)
    {
        try
        {
            var fullPath = Path.Combine(BasePath, relativePath);
            if (!File.Exists(fullPath))
            {
                return false;
            }

            var content = File.ReadAllBytes(fullPath);
            var extension = Path.GetExtension(fullPath).ToLowerInvariant();
            var contentType = extension switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".svg" => "image/svg+xml",
                _ => "application/octet-stream"
            };

            attachments.Add(new EmailAttachment
            {
                ContentId = contentId,
                Content = content,
                ContentType = contentType,
                IsInline = true,
                FileName = Path.GetFileName(fullPath)
            });

            return true;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error loading local attachment {Path}", relativePath);
            return false;
        }
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Processing order confirmation email to {ToAddress} for order {OrderNumber}")]
    private static partial void LogProcessingOrderConfirmation(ILogger logger, string toAddress, string orderNumber);

    [LoggerMessage(Level = LogLevel.Information, Message = "Order confirmation email sent to {ToAddress} for order {OrderNumber}")]
    private static partial void LogOrderConfirmationSent(ILogger logger, string toAddress, string orderNumber);

    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to send order confirmation email to {ToAddress} for order {OrderNumber}")]
    private static partial void LogOrderConfirmationFailed(ILogger logger, Exception ex, string toAddress, string orderNumber);
}
