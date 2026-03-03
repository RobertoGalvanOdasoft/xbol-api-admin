using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.DTO;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/venue-maps")]
    [ApiController]
    public class VenueMapsController(VenueMapService venueMapService) : ControllerBase
    {
        /// <summary>
        /// Retrieves a collection of venue maps.
        /// </summary>
        /// <remarks>This method calls the underlying venue map service to obtain the data. Ensure that
        /// the service is properly configured to return the expected results. The response is returned with an HTTP 200
        /// status code on success.</remarks>
        /// <returns>An ActionResult containing a list of objects that represent the available venue maps.
        /// The list will be empty if no venue maps are found.</returns>
        [HttpGet]
        [EndpointName("GetVenueMapsAsync")]
        [ProducesResponseType(typeof(List<VenueMapListItemDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<VenueMapListItemDTO>>> GetVenueMapsAsync()
        {
            var result = await venueMapService.GetVenueMapListAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves the venue map details for the specified identifier.
        /// </summary>
        /// <remarks>This method performs an asynchronous lookup for the venue map. If no venue map is
        /// found for the given identifier, the response will be 404 Not Found.</remarks>
        /// <param name="venueMapId">The unique identifier of the venue map to retrieve. Must be a positive long value.</param>
        /// <returns>An ActionResult containing a VenueMapListItemDTO if a venue map with the specified identifier exists;
        /// otherwise, a 404 Not Found response.</returns>
        [HttpGet("{venueMapId:long}")]
        [EndpointName("GetVenueMapsByIdAsync")]
        [ProducesResponseType(typeof(VenueMapListItemDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
