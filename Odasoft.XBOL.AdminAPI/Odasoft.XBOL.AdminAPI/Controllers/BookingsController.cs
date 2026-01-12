using Microsoft.AspNetCore.Mvc;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController(ITicketingClient ticketingClient) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<BookingResult>> CreateBooking([FromBody] BookingRequest request)
        {
            var result = await ticketingClient.BookingAsync(request);

            return Ok(new BookingResult { Message = "Booking created successfully", Tickets = result });
        }

        // TODO: Move to an appropiate location
        public class BookingResult
        {
            public long? BookingId { get; set; }
            public string Message { get; set; }
            public IEnumerable<string> Tickets { get; set; } = [];
        }
    }
}
