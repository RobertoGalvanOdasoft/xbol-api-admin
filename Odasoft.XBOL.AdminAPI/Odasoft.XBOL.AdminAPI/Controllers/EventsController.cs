using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.DTO;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/events")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly ITicketingClient _ticketingClient;
        private readonly EventService _eventService;

        public EventsController(ITicketingClient ticketingClient, EventService eventService)
        {
            _ticketingClient = ticketingClient;
            _eventService = eventService;
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
        public async Task<ActionResult<EventInfoDTO>> GetEventByIdAsync([FromRoute] long eventId)
        {
            var result = await _eventService.GetEventByIdAsync(eventId);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
