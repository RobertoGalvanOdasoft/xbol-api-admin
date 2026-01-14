using Microsoft.AspNetCore.Mvc;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController(ITicketingClient ticketingClient) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PagedResponseOfEventListItem>> GetEvents(
            [FromQuery] string? venues,
            [FromQuery] string? categories,
            [FromQuery] DateTimeOffset? startDate,
            [FromQuery] DateTimeOffset? endDate,
            [FromQuery] string? search,
            [FromQuery] string? sortBy,
            [FromQuery] bool? descending,
            [FromQuery] int? page,
            [FromQuery] int? pageSize)
        {
            var result = await ticketingClient.EventsAsync(
                venues, categories, startDate, endDate, search, sortBy, descending, page, pageSize);

            return Ok(result);
        }
    }
}
