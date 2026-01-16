using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business;
using Odasoft.XBOL.DTO.Results;
using Wolverine;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/bookings")]
    [ApiController]
    public class BookingsController(IMessageBus bus) : ControllerBase
    {
        /// <summary>
        /// Creates a new booking based on the specified booking request.
        /// </summary>
        /// <param name="request">The booking details to use for creating the new booking. Cannot be null.</param>
        /// <returns>An ActionResult containing a BookingResult that indicates the outcome of the booking operation.</returns>
        [HttpPost]
        public async Task<ActionResult<BookingResult>> CreateBookingAsync([FromBody] BookingRequest request)
        {
            var result = await bus.InvokeAsync<BookingResult>(new CreateBookingCommand(request));
            return Ok(result);
        }
    }
}
