using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/venue-maps")]
    [ApiController]
    public class VenueMapsController(Business.ITicketingClient ticketingClient) : ControllerBase
    {
        [HttpGet]
        [EndpointName("GetVenueMapsAsync")]
        public async Task<ActionResult<ICollection<VenueMapListItem>>> GetVenueMapsAsync()
        {
            var result = await ticketingClient.GetVenueMapsAsync();

            return Ok(result);
        }

        [HttpGet("{venueMapId}")]
        [EndpointName("GetVenueMapsByIdAsync")]
        public async Task<ActionResult<VenueMapListItem>> GetVenueMapsByIdAsync([FromRoute] long venueMapId)
        {
            var result = await ticketingClient.GetVenueMapByIdAsync(venueMapId);
            return Ok(result);
        }
    }
}
