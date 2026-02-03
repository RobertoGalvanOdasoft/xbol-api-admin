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
        [EndpointName("GetEventsAsync")]
        public async Task<ActionResult<EventListItemPagedResponse>> GetEventsAsync(
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
            var result = await _ticketingClient.GetEventsAsync(
                venues, categories, startDate, endDate, search, sortBy, descending, page, pageSize);

            return Ok(result);
        }

        [HttpGet("{eventId}")]
        [EndpointName("GetEventByIdAsync")]
        public async Task<ActionResult<EventListItem>> GetEventByIdAsync([FromRoute] long eventId)
        {
            var result = await _ticketingClient.GetEvenByIdAsync(eventId);
            return Ok(result);
        }
    }
}
