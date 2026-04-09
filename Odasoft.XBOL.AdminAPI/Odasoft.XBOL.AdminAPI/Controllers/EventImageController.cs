using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.Commons.Enums;
using Odasoft.XBOL.DTO.Responses;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/event-images")]
    [ApiController]
    public class EventImageController : ControllerBase
    {
        private readonly EventImageService _eventImageService;

        public EventImageController(EventImageService eventImageService)
        {
            _eventImageService = eventImageService;
        }

        /// <summary>
        /// Retrieves a list of the event's images.
        /// </summary>
        /// <param name="eventId">The unique Id of the event.</param>
        /// <returns>An ActionResult containing a list of objects representing the available images.</returns>
        [HttpGet("{eventId:long}")]
        [EndpointName("GetEventImagesAsync")]
        [ProducesResponseType(typeof(List<EventImageResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<EventImageResponse>>> GetEventImagesAsync([FromRoute] long eventId)
        {
            List<EventImageResponse> images = await _eventImageService.GetEventImagesAsync(eventId);

            return Ok(images);
        }

        /// <summary>
        /// Retrieves a list of the event`s images filtered by image type.
        /// </summary>
        /// <param name="eventId">The unique Id of the event.</param>
        /// <param name="imageType">The ImageType of the images wanted.</param>
        /// <returns>An ActionResult contaning a list of objects representing the filtered images by ImageType.</returns>
        [HttpGet("{eventId:long}/image-type/{imageType}")]
        [EndpointName("GetEventImagesByImageTypeAsync")]
        [ProducesResponseType(typeof(List<EventImageResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<EventImageResponse>>> GetEventImagesByImageTypeAsync([FromRoute] long eventId, [FromRoute] ImageType imageType)
        {
            List<EventImageResponse> images = await _eventImageService.GetEventImagesByImageTypeAsync(eventId, imageType);

            return Ok(images);
        }

        /// <summary>
        /// Creates a new event image with the specified info in each request.
        /// </summary>
        /// <param name="eventId">The unique Id of the Event.</param>
        /// <param name="imageType">The type of the image.</param>
        /// <param name="order">Order of the image when display.</param>
        /// <param name="imageFile">Image content.</param>
        /// <returns>An ActionResult confirming if the images were created successfully.</returns>
        [HttpPost("{eventId:long}")]
        [EndpointName("UploadEventImageAsync")]
        [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<long>> UploadEventImageAsync([FromRoute] long eventId, [FromForm] ImageType imageType, [FromForm] int order, IFormFile imageFile)
        {
            try
            {
                var result = await _eventImageService.CreateEventImagesAsync(eventId, imageType, order, imageFile);

                if (result > 0)
                {
                    return CreatedAtAction("GetEventImages", new { eventId }, result);
                }

                return UnprocessableEntity("Unable to save image.");
            }
            catch (Exception ex)
            {
                return UnprocessableEntity(ex.Message);
            }
        }

        /// <summary>
        /// Updates an exisiting event image with the specified details.
        /// </summary>
        /// <param name="eventImageId">The unique Id of the Event.</param>
        /// <param name="imageType">The type of the image.</param>
        /// <param name="order">Order of the image when display.</param>
        /// <param name="imageFile">Image content.</param>
        /// <returns>An ActionResult confirming if the event image was updated successfully.</returns>
        [HttpPut("{eventImageId:long}")]
        [EndpointName("UpdateEventImageByIdAsync")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<bool>> UpdateEventImageByIdAsync([FromRoute] long eventImageId, [FromForm] ImageType imageType, [FromForm] int order, IFormFile imageFile)
        {
            var result = await _eventImageService.UpdateEventImageByIdAsync(eventImageId, imageType, order, imageFile);

            if (result)
            {
                return NoContent();
            }

            return UnprocessableEntity("Unable to update event image.");
        }

        /// <summary>
        /// Deletes and existing event image with the specified Id.
        /// </summary>
        /// <param name="eventImageId">The unique Event Image Id.</param>
        /// <returns>An ActionResult confirming if the event image was delted successfully.</returns>
        [HttpDelete("{eventImageId:long}")]
        [EndpointName("DeleteEventImageByIdAsync")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<bool>> DeleteEventImageByIdAsync([FromRoute] long eventImageId)
        {
            var result = await _eventImageService.DeleteEventImageByIdAsync(eventImageId);

            if (result)
            {
                return NoContent();
            }

            return UnprocessableEntity("Unable to delete event Image.");
        }
    }
}
