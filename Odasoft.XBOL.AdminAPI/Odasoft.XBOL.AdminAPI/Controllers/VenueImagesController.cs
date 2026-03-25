using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.Commons.Enums;
using Odasoft.XBOL.DTO.Responses;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/venue-images")]
    [ApiController]
    public class VenueImagesController : ControllerBase
    {
        private readonly VenueImageService _venueImageService;

        public VenueImagesController(VenueImageService venueImageService)
        {
            _venueImageService = venueImageService;
        }

        /// <summary>
        /// Retrieves a list of the venue's images.
        /// </summary>
        /// <param name="venueId">The unique Id of the venue.</param>
        /// <returns>An ActionResult containing a list of objects representing the available images.</returns>
        [HttpGet("{venueId:long}")]
        [EndpointName("GetVenueImagesAsync")]
        [ProducesResponseType(typeof(List<VenueImageResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<VenueImageResponse>>> GetVenueImagesAsync([FromRoute] long venueId)
        {
            List<VenueImageResponse> images = await _venueImageService.GetVenueImagesAsync(venueId);

            return Ok(images);
        }

        /// <summary>
        /// Retrieves a list of the venue`s images filtered by image type.
        /// </summary>
        /// <param name="venueId">The unique Id of the venue.</param>
        /// <param name="imageType">The ImageType of the images wanted.</param>
        /// <returns>An ActionResult contaning a list of objects representing the filtered images by ImageType.</returns>
        [HttpGet("{venueId:long}/image-type/{imageType}")]
        [EndpointName("GetVenueImagesByImageTypeAsync")]
        [ProducesResponseType(typeof(List<VenueImageResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<VenueImageResponse>>> GetVenueImagesByImageTypeAsync([FromRoute] long venueId, [FromRoute] ImageType imageType)
        {
            List<VenueImageResponse> images = await _venueImageService.GetVenueImagesByImageTypeAsync(venueId, imageType);

            return Ok(images);
        }

        /// <summary>
        /// Creates a new venue image with the specified info in each request.
        /// </summary>
        /// <param name="venueId">The unique Id of the Venue.</param>
        /// <param name="imageType">The type of the image.</param>
        /// <param name="order">Order of the image when display.</param>
        /// <param name="imageFile">Image content.</param>
        /// <returns>An ActionResult confirming if the images were created successfully.</returns>
        [HttpPost("{venueId:long}")]
        [EndpointName("UploadVenueImageAsync")]
        [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<long>> UploadVenueImageAsync([FromRoute] long venueId, [FromForm] ImageType imageType, [FromForm] int order, IFormFile imageFile)
        {
            try
            {
                var result = await _venueImageService.CreateVenueImagesAsync(venueId, imageType, order, imageFile);

                if (result > 0)
                {
                    return CreatedAtAction("GetVenueImages", new { venueId }, result);
                }

                return UnprocessableEntity("Unable to save image.");
            }
            catch (Exception ex)
            {
                return UnprocessableEntity(ex.Message);
            }
        }

        /// <summary>
        /// Updates an exisiting venue image with the specified details.
        /// </summary>
        /// <param name="venueImageId">The unique Id of the Venue.</param>
        /// <param name="imageType">The type of the image.</param>
        /// <param name="order">Order of the image when display.</param>
        /// <param name="imageFile">Image content.</param>
        /// <returns>An ActionResult confirming if the venue image was updated successfully.</returns>
        [HttpPut("{venueImageId:long}")]
        [EndpointName("UpdateVenueImageByIdAsync")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<bool>> UpdateVenueImageByIdAsync([FromRoute] long venueImageId, [FromForm] ImageType imageType, [FromForm] int order, IFormFile imageFile)
        {
            var result = await _venueImageService.UpdateVenueImageByIdAsync(venueImageId, imageType, order, imageFile);

            if (result)
            {
                return NoContent();
            }

            return UnprocessableEntity("Unable to update venue Image.");
        }

        /// <summary>
        /// Deletes and existing venue image with the specified Id.
        /// </summary>
        /// <param name="venueImageId">The unique Venue Image Id.</param>
        /// <returns>An ActionResult confirming if the venue image was delted successfully.</returns>
        [HttpDelete("{venueImageId:long}")]
        [EndpointName("DeleteVenueImageByIdAsync")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<bool>> DeleteVenueImageByIdAsync([FromRoute] long venueImageId)
        {
            var result = await _venueImageService.DeleteVenueImageByIdAsync(venueImageId);

            if (result)
            {
                return NoContent();
            }

            return UnprocessableEntity("Unable to delete venue Image.");
        }
    }
}
