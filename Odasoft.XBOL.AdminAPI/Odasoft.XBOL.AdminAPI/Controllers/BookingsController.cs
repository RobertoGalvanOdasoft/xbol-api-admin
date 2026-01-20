using Microsoft.AspNetCore.Mvc;
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
        /// Books the specified seat selection for the event and returns the identifiers of the booked seats.
        /// </summary>
        /// <param name="request">The booking request containing event and seat selection details. Cannot be null.</param>
        /// <returns>An action result containing a collection of strings that represent the keys of the successfully booked
        /// seats.</returns>
        [HttpPost("book-seats")]
        [EndpointName("BookSeatsAsync")]
        public async Task<ActionResult<BookingResult>> BookSeatsAsync([FromBody] BookingRequest request)
        {
            var result = await bus.InvokeAsync<BookingResult>(new CreateBookingCommand(request));
            return Ok(result);
        }
    }
}
