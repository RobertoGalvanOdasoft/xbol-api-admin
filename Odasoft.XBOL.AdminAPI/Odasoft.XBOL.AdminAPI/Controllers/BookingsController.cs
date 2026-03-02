using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Odasoft.XBOL.Business;
using Odasoft.XBOL.Business.Messages;
using Odasoft.XBOL.DTO.Results;
using Wolverine;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/bookings")]
    [ApiController]
    public class BookingsController(IMessageBus bus) : ControllerBase
    {
        /// <summary>
        /// Books seats for an event based on the provided booking request.
        /// </summary>
        /// <remarks>This method processes the booking asynchronously and returns appropriate HTTP status
        /// codes based on the outcome of the booking request.</remarks>
        /// <param name="request">The event booking request containing details such as event ID and number of seats to book. This parameter
        /// cannot be null.</param>
        /// <returns>A BookingResult object that contains the details of the booking operation, including confirmation of the
        /// booked seats.</returns>
        [HttpPost("event/book-seats")]
        [EndpointName("BookEventSeatsAsync")]
        [ProducesResponseType(typeof(BookingResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<BookingResult>> BookSeatsAsync([FromBody] EventBookingRequest request)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var result = await bus.InvokeAsync<BookingResult>(new CreateEventBookingCommand(request));

            if (result is null)
            {
                return UnprocessableEntity("Booking failed. Please check the request details and try again.");
            }

            return Ok(result);
        }

        /// <summary>
        /// Books seats for an event based on the provided booking request.
        /// </summary>
        /// <remarks>This method processes the booking asynchronously and returns appropriate HTTP status
        /// codes based on the outcome of the booking request.</remarks>
        /// <param name="request">The event booking request containing details such as event ID and number of seats to book. This parameter
        /// cannot be null.</param>
        /// <returns>A BookingResult object that contains the details of the booking operation, including confirmation of the
        /// booked seats.</returns>
        [HttpPost("season/book-season")]
        [EndpointName("BookSeasonSeatsAsync")]
        [ProducesResponseType(typeof(BookingResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<List<string>>> BookSeasonSeatsAsync([FromBody] SeasonBookingRequest request)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var result = await bus.InvokeAsync<BookingResult>(new CreateSeasonBookingCommand(request));

            if (result is null)
            {
                return UnprocessableEntity("Booking failed. Please check the request details and try again.");
            }

            return Ok(result);
        }
    }
}
