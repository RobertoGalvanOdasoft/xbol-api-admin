using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.Commons.Email;
using Odasoft.XBOL.Commons.Requests;

namespace Odasoft.XBOL.AdminAPI.Controllers;

/// <summary>
/// Development-only endpoints for testing infrastructure.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TestsController(
    IBackgroundJobClient backgroundJobClient,
    IHostEnvironment environment,
    ITemplateService templateService,
    OrderService orderService) : ControllerBase
{
    /// <summary>
    /// Enqueues a test email via Hangfire.
    /// </summary>
    [HttpPost("email")]
    [EndpointName("TestEmail")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult TestEmail()
    {
        if (!environment.IsDevelopment())
        {
            return NotFound();
        }

        var model = new TestEmailModel
        {
            ToAddress = "test@example.com",
            ToName = "Test User",
            Subject = "XBOL Test Email",
            Name = "Test User"
        };

        var jobId = backgroundJobClient.Enqueue<IEmailJob>(x => x.SendTestEmailAsync(model));

        return Ok(new { message = "Test email enqueued.", jobId });
    }

    /// <summary>
    /// Enqueues an order confirmation email via Hangfire using real order data.
    /// </summary>
    [HttpPost("email/order-confirmation/{orderId:long}")]
    [EndpointName("TestOrderConfirmationEmail")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TestOrderConfirmationEmail(long orderId, [FromQuery] string culture = "es-MX")
    {
        if (!environment.IsDevelopment())
        {
            return NotFound();
        }

        var model = await orderService.BuildOrderConfirmationAsync(orderId, "test@example.com", "Test User", culture);
        var jobId = backgroundJobClient.Enqueue<IEmailJob>(x => x.SendOrderConfirmationAsync(model));

        return Ok(new { message = "Order confirmation email enqueued.", jobId });
    }

    /// <summary>
    /// Renders the order confirmation email template as HTML for design iteration.
    /// </summary>
    [HttpGet("email/order-confirmation/{orderId:long}/preview")]
    [EndpointName("PreviewOrderConfirmationEmail")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PreviewOrderConfirmationEmail(long orderId, [FromQuery] string culture = "es-MX")
    {
        if (!environment.IsDevelopment())
        {
            return NotFound();
        }

        templateService.ClearCache();
        var model = await orderService.BuildOrderConfirmationAsync(orderId, "test@example.com", "Test User", culture);
        var html = await templateService.RenderAsync("OrderConfirmation", model);

        return Content(html, "text/html");
    }
}
