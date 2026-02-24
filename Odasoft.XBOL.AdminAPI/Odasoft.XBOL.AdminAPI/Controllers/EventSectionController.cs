using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.DTO;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/event-sections")]
    [ApiController]
    public class EventSectionController(EventSectionService _eventSectionService) : Controller
    {
        /// <summary>
        /// Retrieves the list of zone prices for the specified event.
        /// </summary>
        /// <param name="eventId">The unique identifier of the event for which to retrieve zone prices.</param>
        /// <returns>An asynchronous operation that returns an object containing the
        /// list of zone prices for the event.</returns>
        [HttpGet("{eventId}/zone-prices")]
        [EndpointName("GetZonePricesByEventIdAsync")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<ZonePriceDTO>))]
        public async Task<ActionResult<List<ZonePriceDTO>>> GetZonePricesByEventIdAsync([FromRoute] long eventId)
        {
            IList<ZonePriceDTO> result = await _eventSectionService.GetZonePricesByEventIdAsync(eventId);

            return Ok(result);
        }
    }
}
