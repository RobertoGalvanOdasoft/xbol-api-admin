using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/venues")]
    [ApiController]
    public class VenuesController(ITicketingClient ticketingClient) : ControllerBase
    {
        [HttpGet]
        [EndpointName("GetVenuesAsync")]
        public async Task<ActionResult<ICollection<VenueListItem>>> GetVenuesAsync()
        {
            var result = await ticketingClient.GetVenuesAsync();

            return Ok(result);
        }
    }
}
