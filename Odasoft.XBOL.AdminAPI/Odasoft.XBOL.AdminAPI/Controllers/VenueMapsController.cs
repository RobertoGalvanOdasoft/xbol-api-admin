using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VenueMapsController(ITicketingClient ticketingClient) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<ICollection<VenueMapListItem>>> GetVenueMaps()
        {
            var result = await ticketingClient.VenueMapsAsync();

            return Ok(result);
        }
    }
}
