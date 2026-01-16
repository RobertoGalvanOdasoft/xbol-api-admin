using Microsoft.AspNetCore.Mvc;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/venue-maps")]
    [ApiController]
    public class VenueMapsController(ITicketingClient ticketingClient) : ControllerBase
    {
        [HttpGet]
        [EndpointName("GetVenueMaps")]
        public async Task<ActionResult<ICollection<VenueMapListItem>>> GetVenueMaps()
        {
            var result = await ticketingClient.GetVenueMapsAsync();

            return Ok(result);
        }

        [HttpGet("{id}")]
        [EndpointName("GetVenueMap")]
        public async Task<ActionResult<VenueMapListItem>> GetVenueMap([FromRoute] long id)
        {
            var result = await ticketingClient.GetVenueMapAsync(id);
            return Ok(result);
        }
    }
}
