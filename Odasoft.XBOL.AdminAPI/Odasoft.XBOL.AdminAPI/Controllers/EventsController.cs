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

        /// <summary>
        /// Retrieves the event catalog as a collection of list items.
        /// </summary>
        /// <returns>An object containing the event catalog. The collection will
        /// be empty if no items are available.</returns>
        [HttpGet("catalog")]
        [EndpointName("GetEventCatalogAsync")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<ListItem>))]
        public async Task<ActionResult<ICollection<ListItem>>> GetEventCatalogAsync()
        {
            var result = await eventService.GetEventCatalogAsync();
            return Ok(result);
        }
    }
}
