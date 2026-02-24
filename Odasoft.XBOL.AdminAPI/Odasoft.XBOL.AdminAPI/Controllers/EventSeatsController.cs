using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.DTO;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/event-seats")]
    [ApiController]
    public class EventSeatsController : ControllerBase
    {
        private readonly EventSeatsService _eventSeatService;

        public EventSeatsController(EventSeatsService eventSeatService)
        {
            _eventSeatService = eventSeatService;
        }

        /// <summary>
        /// Retrieves the list of seat prices for the specified event.
        /// </summary>
        /// <param name="eventId">The unique identifier of the event for which to retrieve seat prices.</param>
        /// <returns>An asynchronous operation that returns an object containing the
        /// list of seat prices for the event.</returns>
        [HttpGet("{eventId}/seat-prices")]
        [EndpointName("GetSeatPricesByEventIdAsync")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<SeatPriceDTO>))]
        public async Task<ActionResult<List<SeatPriceDTO>>> GetSeatPricesByEventIdAsync([FromRoute] long eventId)
        {
            IList<SeatPriceDTO> result = await _eventSeatService.GetSeatPricesByEventIdAsync(eventId);

            return Ok(result);
        }
    }
}
