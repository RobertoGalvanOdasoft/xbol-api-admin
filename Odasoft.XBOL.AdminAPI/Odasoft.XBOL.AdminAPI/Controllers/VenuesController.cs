using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.Commons.Enums;
using Odasoft.XBOL.DTO;
using Odasoft.XBOL.DTO.QueryParams;
using Odasoft.XBOL.DTO.Requests;
using Odasoft.XBOL.DTO.Response;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/venues")]
    [ApiController]
    public class VenuesController(VenueService venueService) : ControllerBase
    {
        /// <summary>
        /// Retrieves a list of venues asynchronously.
        /// </summary>
        /// <remarks>This method calls the venue service to obtain the venue list. The response will
        /// contain the venue data if the operation is successful.</remarks>
        /// <returns>An ActionResult containing a list of objects representing the available venues.
        /// Returns an HTTP 200 OK response with the venue data if successful.</returns>
        [HttpGet]
        [EndpointName("GetVenuesAsync")]
        [ProducesResponseType(typeof(PagedResponse<VenueResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<VenueResponse>>> GetVenuesAsync([FromQuery] VenueQueryParams queryParams)
        {
            var result = await venueService.GetVenueListAsync(queryParams);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a venue with the specified identifier.
        /// </summary>
        /// <param name="venueId">The unique identifier of the venue.</param>
        /// <returns>An ActionResult containing an object with the venue information.</returns>
        [HttpGet("{venueId:long}")]
        [EndpointName("GetVenueByIdAsync")]
        [ProducesResponseType(typeof(VenueResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VenueResponse>> GetVenueByIdAsync([FromRoute] long venueId)
        {
            var result = await venueService.GetVenueByIdAsync(venueId);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Retrieves a collection of venue catalog items for use in selection lists or dropdowns.
        /// </summary>
        /// <remarks>Use this endpoint to obtain a list of venues formatted for display in UI components
        /// such as dropdowns. The returned items typically include venue identifiers and display names.</remarks>
        /// <returns>An ActionResult containing the collection of venue catalog items.
        /// Returns an empty collection if no venues are available.</returns>
        [HttpGet("catalog")]
        [EndpointName("GetVenueCatalogAsync")]
        [ProducesResponseType(typeof(List<ListItem>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ListItem>>> GetVenueCatalogAsync()
        {
            var result = await venueService.GetVenueCatalogAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a list of venue cities for use in selection lists or dropdowns.
        /// </summary>
        /// <returns>An ActionResult containing the list of venue cites name.
        /// Returns an empty collection if no venues are available.</returns>
        [HttpGet("cities-catalog")]
        [EndpointName("GetVenueCitiesCatalogAsync")]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<string>>> GetVenueCitiesCatalogAsync()
        {
            var result = await venueService.GetVenueCitiesCatalogAsync();
            return Ok(result);
        }

        /// <summary>
        /// Creates a new Venue using the specified request data.
        /// </summary>
        /// <param name="request">The details of the Venue to create.</param>
        /// <returns>An ActionResult containing the unique Id of the new Venue.</returns>
        [HttpPost]
        [EndpointName("CreateVenueAsync")]
        [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<long>> CreateVenueAsync([FromBody] VenueRequest request)
        {
            var result = await venueService.CreateVenueAsync(request);

            if (result > 0)
            {
                return CreatedAtAction("GetVenueById", new { venueId = result }, result);
            }

            return UnprocessableEntity("Unable to create venue.");
        }

        /// <summary>
        /// Updates an existing Venue with the specified details.
        /// </summary>
        /// <param name="venueId">Unique Id of the Venue.</param>
        /// <param name="request">The request object containing the updated Venue information.</param>
        /// <returns>An ActionResult to determine if the update was successful.</returns>
        [HttpPut("{venueId:long}")]
        [EndpointName("UpdateVenueAsync")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult> UpdateVenueAsync([FromRoute] long venueId, [FromBody] VenueRequest request)
        {
            var result = await venueService.UpdateVenueAsync(venueId, request);

            if (result)
            {
                return NoContent();
            }

            return UnprocessableEntity("Unable to update venue.");
        }

        /// <summary>
        /// Deletes the Venue with the specified Id.
        /// </summary>
        /// <param name="venueId">The unique Id of the Venue.</param>
        /// <returns>An ActionResult to determine if the delete was successful.</returns>
        [HttpDelete("{venueId:long}")]
        [EndpointName("DeleteVenuAsync")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult> DeleteVenueAsync([FromRoute] long venueId)
        {
            var result = await venueService.DeleteVenueAsync(venueId);

            if (result)
            {
                return NoContent();
            }

            return UnprocessableEntity("Unable to delete venue.");
        }

        /// <summary>
        /// Updates the venue status.
        /// </summary>
        /// <param name="venueId">The unique Id of the Venue.</param>
        /// <param name="venueStatus">The new venue status.</param>
        /// <returns></returns>
        [HttpPatch("{venueId:long}/status/{venueStatus}")]
        [EndpointName("UpdateVenueStatusAsync")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult> UpdateVenueStatusAsync([FromRoute] long venueId, [FromRoute] VenueStatus venueStatus)
        {
            var result = await venueService.UpdateVenueStatusAsync(venueId, venueStatus);

            if (result)
            {
                return NoContent();
            }

            return UnprocessableEntity("Unable to update venue.");
        }
    }
}
