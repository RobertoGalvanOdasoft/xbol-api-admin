using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business;
using Odasoft.XBOL.Business.Services;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/events")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly EventService _eventService;
        private readonly ITicketingClient _ticketingClient;

        public EventsController(EventService eventService, ITicketingClient ticketingClient)
        {
            _eventService = eventService;
            _ticketingClient = ticketingClient;
        }

        [HttpGet]
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

        [HttpGet("{id}")]
        public async Task<ActionResult<EventListItem>> GetEventAsync([FromRoute] long id)
        {
            var result = await _ticketingClient.GetEventAsync(id);
            return Ok(result);
        }
    }
}
