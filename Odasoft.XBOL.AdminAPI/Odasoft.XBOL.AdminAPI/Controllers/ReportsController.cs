using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.DTO.Results.Reports;

namespace Odasoft.XBOL.AdminAPI.Controllers;

[Route("api/reports")]
[ApiController]
public class ReportsController(ReportService reportService) : ControllerBase
{
    /// <summary>
    /// Retrieves a summary report of seat availability grouped by section.
    /// </summary>
    /// <param name="key">The event key to generate the report for.</param>
    [HttpGet("{key}/summary/by-section")]
    [EndpointName("GetSectionSummary")]
    [ProducesResponseType(typeof(Dictionary<string, SectionSummaryResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Dictionary<string, SectionSummaryResult>>> GetSectionSummaryAsync(
        [FromRoute] string key)
    {
        var result = await reportService.GetSectionSummaryAsync(key);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a summary report of seat availability grouped by zone.
    /// </summary>
    /// <param name="key">The event key to generate the report for.</param>
    [HttpGet("{key}/summary/by-zone")]
    [EndpointName("GetZoneSummary")]
    [ProducesResponseType(typeof(Dictionary<string, ZoneSummaryResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Dictionary<string, ZoneSummaryResult>>> GetZoneSummaryAsync(
        [FromRoute] string key)
    {
        var result = await reportService.GetZoneSummaryAsync(key);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a summary report of seat availability grouped by availability (available vs not_available).
    /// </summary>
    /// <param name="key">The event key to generate the report for.</param>
    [HttpGet("{key}/summary/by-availability")]
    [EndpointName("GetAvailabilitySummary")]
    [ProducesResponseType(typeof(Dictionary<string, AvailabilitySummaryResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Dictionary<string, AvailabilitySummaryResult>>> GetAvailabilitySummaryAsync(
        [FromRoute] string key)
    {
        var result = await reportService.GetAvailabilitySummaryAsync(key);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a summary report of seat availability grouped by availability reason
    /// (available, booked, reservedByToken, not_for_sale, and any custom statuses).
    /// </summary>
    /// <param name="key">The event key to generate the report for.</param>
    [HttpGet("{key}/summary/by-availability-reason")]
    [EndpointName("GetAvailabilityReasonSummary")]
    [ProducesResponseType(typeof(Dictionary<string, AvailabilityReasonSummaryResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Dictionary<string, AvailabilityReasonSummaryResult>>> GetAvailabilityReasonSummaryAsync(
        [FromRoute] string key)
    {
        var result = await reportService.GetAvailabilityReasonSummaryAsync(key);
        return Ok(result);
    }
}
