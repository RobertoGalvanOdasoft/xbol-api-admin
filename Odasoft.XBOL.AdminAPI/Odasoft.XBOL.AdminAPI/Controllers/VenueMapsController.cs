using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.DTO;
using Odasoft.XBOL.DTO.Requests;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/venue-maps")]
    [ApiController]
    public class VenueMapsController(VenueMapService venueMapService, ITicketingClient ticketingClient) : ControllerBase
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
        [ProducesResponseType(typeof(List<VenueMapResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<VenueMapResponse>>> GetVenueMapsAsync()
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
        [ProducesResponseType(typeof(VenueMapResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VenueMapResponse>> GetVenueMapsByIdAsync([FromRoute] long venueMapId)
        {
            var result = await venueMapService.GetVenueMapByIdAsync(venueMapId);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Creates a new Venue Map or Maps using the specified request data.
        /// </summary>
        /// <param name="requests">The list of details of the Venue Map or Maps to create.</param>
        /// <returns>An ActionResult to confirm it the Venue Map or Maps were created.</returns>
        [HttpPost]
        [EndpointName("CreateVenueMapAsync")]
        [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<long>> CreateVenueAsync(List<VenueMapRequest> requests)
        {
            var result = await venueMapService.CreateVenueMapAsync(requests);

            if (result)
            {
                return CreatedAtAction("GetVenueMapsById", new { venueMapId = result }, result);
            }

            return UnprocessableEntity("Unable to create venue map.");
        }

        /// <summary>
        /// Updates an existing Venue Map with the specified details.
        /// </summary>
        /// <param name="venueMapId">Unique Id of the Venue Map.</param>
        /// <param name="request">The request object containing the updated Venue Map information.</param>
        /// <returns>An ActionResult to determine if the update was successful.</returns>
        [HttpPut("{venueMapId:long}")]
        [EndpointName("UpdateVenueMapAsync")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult> UpdateVenueAsync([FromRoute] long venueMapId, VenueMapRequest request)
        {
            var result = await venueMapService.UpdateVenueMapAsync(venueMapId, request);

            if (result)
            {
                return NoContent();
            }

            return UnprocessableEntity("Unable to update venue map.");
        }

        /// <summary>
        /// Deletes the Venue Map with the specified Id.
        /// </summary>
        /// <param name="venueMapId">The unique Id of the Venue Map.</param>
        /// <returns>An ActionResult to determine if the delete was successful.</returns>
        [HttpDelete("{venueMapId:long}")]
        [EndpointName("DeleteVenueMapAsync")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult> DeleteVenueAsync([FromRoute] long venueMapId)
        {
            var result = await venueMapService.DeleteVenueMapAsync(venueMapId);

            if (result)
            {
                return NoContent();
            }

            return UnprocessableEntity("Unable to delete venue map.");
        }

        [HttpGet("{chartKey}/validate")]
        [EndpointName("GetVenueMapChartByKeyAsync")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> GetVenueMapChartByKeyAsync([FromRoute] string chartKey)
        {
            Chart? chart = await ticketingClient.GetChartByKeyAsync(chartKey);

            if (chart == null)
            {
                return Ok(false);
            }

            return Ok(true);
        }
    }
}
