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
        /// Retrieves a collection of venue maps from the venue provided.
        /// </summary>
        /// <param name="venueId">Unique venue identifier.</param>
        /// <returns>An ActionResult containing a list of objects that represent the available venue maps for the venue.
        /// The list will be empty if no venue maps are found.</returns>
        [HttpGet("/venue/{venueId:long}")]
        [EndpointName("GetVenueMapsByVenueAsync")]
        [ProducesResponseType(typeof(List<VenueMapResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<VenueMapResponse>>> GetVenueMapsAsync([FromRoute] long venueId)
        {
            var result = await venueMapService.GetVenueMapsByVenueAsync(venueId);
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
        [EndpointName("GetVenueMapsById")]
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
        /// Retrieves a collection of venue map catalog items for use in selection lists or dropdowns.
        /// </summary>
        /// <remarks>Use this endpoint to obtain a list of venues formatted for display in UI components
        /// such as dropdowns. The returned items typically include venue map identifiers and display names.</remarks>
        /// <param name="venueId">The unique identifier of the venue id of the maps to retrieve. If null then returns all venue maps</param>
        /// <returns>An ActionResult containing the collection of venue map catalog items.
        /// Returns an empty collection if no venue maps are available.</returns>
        [HttpGet("catalog/{venueId:long}")]
        [EndpointName("GetVenueMapCatalogByVenueIdAsync")]
        [ProducesResponseType(typeof(List<VenueMapResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<VenueMapResponse>>> GetVenueMapCatalogByVenueIdAsync(long? venueId)
        {
            var result = await venueMapService.GetVenueMapCatalogByVenueIdAsync(venueId);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new Venue Map using the specified request data.
        /// </summary>
        /// <param name="request">The details of the Venue Map to create.</param>
        /// <returns>An ActionResult to confirm it the Venue Map was created.</returns>
        [HttpPost]
        [EndpointName("CreateVenueMapAsync")]
        [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<long>> CreateVenueAsync(VenueMapRequest request)
        {
            var result = await venueMapService.CreateVenueMapAsync(request);

            if (result > 0)
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

        /// <summary>
        /// Retrieves venue map chart data using the unique chart key provided.
        /// </summary>
        /// <param name="chartKey">The unique chart key</param>
        /// <returns>Returns a Chart object containing the venue map data.</returns>
        [HttpGet("{chartKey}/validate")]
        [EndpointName("GetVenueMapChartByKeyAsync")]
        [ProducesResponseType(typeof(Chart), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Chart), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Chart>> GetVenueMapChartByKeyAsync([FromRoute] string chartKey)
        {
            if (string.IsNullOrWhiteSpace(chartKey))
            {
                return BadRequest("Chart key must be provided.");
            }

            try
            {
                Chart chart = await ticketingClient.GetChartByKeyAsync(chartKey);

                return Ok(chart);
            }
            catch (ApiException ex)
            {
                if (ex.StatusCode == StatusCodes.Status404NotFound)
                {
                    return NotFound();
                }

                throw;
            }
        }
    }
}
