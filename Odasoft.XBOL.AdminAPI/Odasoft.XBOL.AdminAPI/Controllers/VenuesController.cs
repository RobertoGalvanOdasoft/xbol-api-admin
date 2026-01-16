using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VenuesController(ITicketingClient ticketingClient) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<ICollection<VenueListItem>>> GetVenues()
        {
            var result = await ticketingClient.VenuesAsync();

            return Ok(result);
        }
    }
}
