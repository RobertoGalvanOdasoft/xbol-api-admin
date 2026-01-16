using Microsoft.AspNetCore.Mvc;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/venues")]
    [ApiController]
    public class VenuesController(ITicketingClient ticketingClient) : ControllerBase
    {
        [HttpGet]
        [EndpointName("GetVenues")]
        public async Task<ActionResult<ICollection<VenueListItem>>> GetVenues()
        {
            var result = await ticketingClient.GetVenuesAsync();

            return Ok(result);
        }
    }
}
