using Microsoft.AspNetCore.Mvc;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VenuesController(ITicketingClient ticketingClient) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<ICollection<string>>> GetVenues()
        {
            var result = await ticketingClient.VenuesAsync();

            return Ok(result);
        }
    }
}
