using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.DTO.Results;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/bookings")]
    [ApiController]
    public class BookingsController(ITicketingClient ticketingClient) : ControllerBase
    {
        /// <summary>
        /// Creates a new booking based on the specified booking request.
        /// </summary>
        /// <param name="request">The booking details to use for creating the new booking. Cannot be null.</param>
        /// <returns>An ActionResult containing a BookingResult that indicates the outcome of the booking operation.</returns>
        [HttpPost]
        [EndpointName("CreateBooking")]
        public async Task<ActionResult<BookingResult>> CreateBooking([FromBody] BookingRequest request)
        {
            var result = await ticketingClient.CreateBookingAsync(request);

            return Ok(new BookingResult { Message = "Booking created successfully", Tickets = result });
        }
    }
}
