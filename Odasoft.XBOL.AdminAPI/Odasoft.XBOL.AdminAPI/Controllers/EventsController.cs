using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.Commons.Enums;
using Odasoft.XBOL.DTO;
using Odasoft.XBOL.DTO.Response;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/events")]
    [ApiController]
    public class EventsController(EventService eventService) : ControllerBase
    {
        [HttpGet]
        [EndpointName("GetEventsAsync")]
        public async Task<ActionResult<PagedResponse<EventListItemDTO>>> GetEventsAsync(
            [FromQuery] string? venues,
            [FromQuery] string? categories,
            [FromQuery] DateTimeOffset? startDate,
            [FromQuery] DateTimeOffset? endDate,
            [FromQuery] string? search,
            [FromQuery] string? sortBy,
            [FromQuery] bool? descending,
            [FromQuery] int? page,
            [FromQuery] int? pageSize,
            [FromQuery] long? seasonId,
            [FromQuery] EventStatus? status)
        {
            var result = await eventService.GetEventListAsync(
                venues, categories, startDate, endDate, search, sortBy, descending, page, pageSize,
                seasonId, status);

            return Ok(result);
        }

        [HttpGet("{eventId}")]
        [EndpointName("GetEventByIdAsync")]
        public async Task<ActionResult<EventInfoDTO>> GetEventByIdAsync([FromRoute] long eventId)
        {
            var result = await eventService.GetEventByIdAsync(eventId);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
