using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.DTO;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/event-sections")]
    [ApiController]
    public class EventSectionController(EventSectionService _eventSectionService) : ControllerBase
    {
        /// <summary>
        /// Retrieves the list of zone prices for the specified event.
        /// </summary>
        /// <param name="eventId">The unique identifier of the event for which to retrieve zone prices.</param>
        /// <returns>An asynchronous operation that returns an object containing the
        /// list of zone prices for the event.</returns>
        [HttpGet("{eventId:long}/zone-prices")]
        [EndpointName("GetZonePricesByEventIdAsync")]
        [ProducesResponseType(typeof(List<ZonePriceDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ZonePriceDTO>>> GetZonePricesByEventIdAsync([FromRoute] long eventId)
        {
            List<ZonePriceDTO> result = await _eventSectionService.GetZonePricesByEventIdAsync(eventId);

            return Ok(result);
        }
    }
}
