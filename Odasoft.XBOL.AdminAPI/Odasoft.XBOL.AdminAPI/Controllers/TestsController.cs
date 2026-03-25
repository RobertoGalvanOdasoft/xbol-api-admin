using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Commons.Email;
using Odasoft.XBOL.Commons.Requests;

namespace Odasoft.XBOL.AdminAPI.Controllers;

/// <summary>
/// Development-only endpoints for testing infrastructure.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TestsController(IBackgroundJobClient backgroundJobClient, IHostEnvironment environment) : ControllerBase
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
            return NotFound();

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
}
