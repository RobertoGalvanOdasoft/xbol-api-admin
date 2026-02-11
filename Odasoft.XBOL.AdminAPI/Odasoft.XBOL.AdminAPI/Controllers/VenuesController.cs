using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.DTO;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/venues")]
    [ApiController]
    public class VenuesController(VenueService venueService) : ControllerBase
    {
        [HttpGet]
        [EndpointName("GetVenuesAsync")]
        public async Task<ActionResult<ICollection<VenueListItemDTO>>> GetVenuesAsync()
        {
            var result = await venueService.GetVenueListAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a collection of venue catalog items for use in selection lists or dropdowns.
        /// </summary>
        /// <remarks>Use this endpoint to obtain a list of venues formatted for display in UI components
        /// such as dropdowns. The returned items typically include venue identifiers and display names.</remarks>
        /// <returns>An object containing the collection of venue catalog items.
        /// Returns an empty collection if no venues are available.</returns>
        [HttpGet("catalog")]
        [EndpointName("GetVenueCatalogAsync")]
        public async Task<ActionResult<ICollection<ListItem>>> GetVenueCatalogAsync()
        {
            var result = await venueService.GetVenueCatalogAsync();
            return Ok(result);
        }
    }
}
