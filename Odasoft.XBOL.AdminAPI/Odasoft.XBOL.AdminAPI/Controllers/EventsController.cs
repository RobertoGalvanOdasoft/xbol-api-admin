using Microsoft.AspNetCore.Mvc;
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
        [EndpointName("GetEvents")]
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
            var result = await _ticketingClient.GetEventsAsync(
                venues, categories, startDate, endDate, search, sortBy, descending, page, pageSize);

            return Ok(result);
        }

        [HttpGet("{id}")]
        [EndpointName("GetEvent")]
        public async Task<ActionResult<EventListItem>> GetEvent([FromRoute] long id)
        {
            var result = await _ticketingClient.GetEventAsync(id);
            return Ok(result);
        }
    }
}