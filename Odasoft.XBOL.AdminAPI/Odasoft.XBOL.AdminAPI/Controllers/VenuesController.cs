using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.DTO;

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
        [ProducesResponseType(typeof(List<VenueListItemDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<VenueListItemDTO>>> GetVenuesAsync()
        {
            var result = await venueService.GetVenueListAsync();
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
    }
}
