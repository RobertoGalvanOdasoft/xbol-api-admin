using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.DTO.Responses;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/venue-amenities")]
    [ApiController]
    public class VenueAmenitiesController : ControllerBase
    {
        private readonly AmenityService _amenityService;
        private readonly VenueService _venueService;
        private readonly VenueAmenityService _venueAmenityService;

        public VenueAmenitiesController(
            AmenityService amenityService,
            VenueService venueService,
            VenueAmenityService venueAmenityService)
        {
            _amenityService = amenityService;
            _venueService = venueService;
            _venueAmenityService = venueAmenityService;
        }

        /// <summary>
        /// Retrieves the list of amenities available for the specified venue.
        /// </summary>
        /// <param name="venueId">The unique identifier of the venue for which to retrieve amenities.</param>
        /// <returns>An ActionResult containing a list of amenities of the venue. <returns>
        [HttpGet("{venueId:long}/amenities")]
        [EndpointName("GetAmenitiesByVenueAsync")]
        [ProducesResponseType(typeof(List<AmenityResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetAmenitiesByVenueAsync([FromRoute] long venueId)
        {
            var result = await _venueService.GetAmenityByVenueAsync(venueId);

            if (result == null)
            {
                return Ok(new List<AmenityResponse>());
            }

            return Ok(result);
        }

        // <summary>
        /// Retrieves a list of amenities for use in selection lists or dropdowns.
        /// </summary>
        /// <returns>An ActionResult containing the list amenity object.
        /// Returns an empty collection if no amenities are available.</retu
        [HttpGet("/amenities-catalog")]
        [EndpointName("GetAmenitiesCatalogAsync")]
        [ProducesResponseType(typeof(List<AmenityResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetAmenitiesAsync()
        {
            var amenities = await _amenityService.GetAmenitiesAsync();

            if (amenities == null)
            {
                return Ok(new List<AmenityResponse>());
            }

            return Ok(amenities);
        }

        [HttpPost("{venueId:long}/amenities")]
        [EndpointName("SaveVenueAmenitiesAsync")]
        [ProducesResponseType(typeof(List<long>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> SaveVenueAmenitiesAsync([FromRoute] long venueId, [FromBody] List<long> amenityIds)
        {
            var venue = await _venueService.GetVenueByIdAsync(venueId);

            if (venue == null)
            {
                return NotFound();
            }

            var result = await _venueAmenityService.SaveVenueAmenitiesAsync(venueId, amenityIds);
            return Ok(result);
        }
    }
}
