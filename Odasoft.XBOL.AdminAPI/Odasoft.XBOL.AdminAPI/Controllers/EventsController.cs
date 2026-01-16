using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/events")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly ITicketingClient _ticketingClient;

        public EventsController(ITicketingClient ticketingClient)
        {
            _ticketingClient = ticketingClient;
        }

        [HttpGet]
        public async Task<ActionResult<EventListItemPagedResponse>> GetEvents(
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
            var result = await _ticketingClient.EventsAsync(
                venues, categories, startDate, endDate, search, sortBy, descending, page, pageSize);

            return Ok(result);
        }
    }
}
