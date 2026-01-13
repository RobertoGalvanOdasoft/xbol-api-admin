using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.DTO;
using Odasoft.XBOL.DTO.Results;

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
        /// <returns>An asynchronous operation that returns an <see cref="ActionResult{IList{SeatPriceDTO}}"/> containing the
        /// list of seat prices for the event.</returns>
        [HttpGet("{eventId}/seat-prices")]
        public async Task<ActionResult<IList<SeatPriceDTO>>> GetSeatPricesAsync([FromRoute] long eventId)
        {
            IList<SeatPriceDTO> result = await _eventSeatService.GetSeatPricesForEventAsync(eventId);

            return Ok(new SeatPricesResult { Message = "List of seat prices", SeatPrices = result });
        }
    }
}
