using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.DTO;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/venue-maps")]
    [ApiController]
    public class VenueMapsController(VenueMapService venueMapService) : ControllerBase
    {
        [HttpGet]
        [EndpointName("GetVenueMapsAsync")]
        public async Task<ActionResult<ICollection<VenueMapListItemDTO>>> GetVenueMapsAsync()
        {
            var result = await venueMapService.GetVenueMapListAsync();
            return Ok(result);
        }

        [HttpGet("{venueMapId}")]
        [EndpointName("GetVenueMapsByIdAsync")]
        public async Task<ActionResult<VenueMapListItemDTO>> GetVenueMapsByIdAsync([FromRoute] long venueMapId)
        {
            var result = await venueMapService.GetVenueMapByIdAsync(venueMapId);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
