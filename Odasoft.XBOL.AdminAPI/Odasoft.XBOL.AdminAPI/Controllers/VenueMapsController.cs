using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/venue-maps")]
    [ApiController]
    public class VenueMapsController(ITicketingClient ticketingClient) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<ICollection<VenueMapListItem>>> GetVenueMapsAsync()
        {
            var result = await ticketingClient.GetVenueMapsAsync();

            return Ok(result);
        }

        [HttpGet("{venueMapId}")]
        public async Task<ActionResult<VenueMapListItem>> GetVenueMapsByIdAsync([FromRoute] long id)
        {
            var result = await ticketingClient.GetVenueMapAsync(id);
            return Ok(result);
        }
    }
}
