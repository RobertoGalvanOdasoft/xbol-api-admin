using Microsoft.Extensions.Localization;
using Odasoft.XBOL.Commons.Constants;
using Odasoft.XBOL.Commons.Email;
using Odasoft.XBOL.Commons.Requests;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using QRCoder;
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

    public async Task SendOrderEmailAsync(OrderEmailModel model, string template, bool generateTickets)
    {
        try
        {
            LogProcessing(logger, model.ToAddress);
            var htmlBody = await templateService.RenderAsync(template, model);

            // TODO: Should move ticket generation to a separate service and generate before rendering template, then pass generated ticket info to template for rendering
            if (generateTickets)
            {
                List<EmailAttachment> emailAttachments = new List<EmailAttachment>();

                foreach (SeatInfo seat in model.Seats)
                {
                    byte[] pdfBytes = await GenerateTicketAsync(new TicketModel
                    {
                        Event = model.EventTitle,
                        OrderReference = model.OrderDetails.OrderNumber,
                        SeatKey = seat.SeatKey
                    });

                    emailAttachments.Add(new EmailAttachment
                    {
                        ContentId = $"ticket-{seat.SeatKey}",
                        Content = pdfBytes,
                        ContentType = "application/pdf",
                        FileName = $"Ticket_{model.OrderDetails.OrderNumber}_{seat.SeatKey}.pdf",
                        IsInline = false
                    });
                }

                await emailService.SendAsync(model.ToAddress, model.ToName, model.Subject, htmlBody, emailAttachments);
                LogSent(logger, model.ToAddress);
                return;
            }

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

    public async Task SendOrderConfirmationAsync(OrderEmailModel model)
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

    private async Task<byte[]> GenerateTicketAsync(TicketModel model)
    {
        model.QrBase64 = GenerateQrBase64(model.SeatKey);

        var htmlBody = await templateService.RenderAsync(EmailTemplateConstants.ORDER_TICKET_ATTACHMENT, model);

        return await GeneratePdfFromHtmlAsync(htmlBody);
    }

    // TODO: Implement the correct way to generate QR code based on the actual data that needs to be encoded
    // Also this should also be generated from API Ticketing instead of being generated locally in the email service,
    // but for demo purposes we can generate a simple QR code based on the seat key or some other unique identifier
    private string GenerateQrBase64(string data)
    {
        using var qrCodeGenerator = new QRCodeGenerator();
        using var qrCodeData = qrCodeGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);

        using var qrCode = new PngByteQRCode(qrCodeData);
        byte[] qrCodeBytes = qrCode.GetGraphic(20);

        return Convert.ToBase64String(qrCodeBytes);
    }

    public async Task<byte[]> GeneratePdfFromHtmlAsync(string html)
    {
        var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();

        await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true,
            Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" }
        });

        await using var page = await browser.NewPageAsync();
        await page.SetContentAsync(html);

        return await page.PdfDataAsync(new PdfOptions
        {
            Format = PaperFormat.A5,
            PrintBackground = true,
            MarginOptions = new MarginOptions
            {
                Top = "10mm",
                Bottom = "10mm",
                Left = "10mm",
                Right = "10mm"
            }
        });
    }
}
