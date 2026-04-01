using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.DTO;
using Odasoft.XBOL.DTO.Requests;
using Odasoft.XBOL.DTO.Results;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/event-schedules")]
    [ApiController]
    public class EventScheduleController(EventScheduleService scheduleService) : ControllerBase
    {
        [HttpGet("{scheduleId:long}")]
        [EndpointName("GetScheduleByIdAsync")]
        [ProducesResponseType(typeof(EventInfoDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EventInfoDTO>> GetScheduleByIdAsync([FromRoute] long scheduleId)
        {
            // TODO: Implementation
            return Ok();
        }
        /// <summary>
        /// Creates a new event schedule using the specified request data and returns the result of the operation.
        /// </summary>
        /// <remarks>If the model state is invalid, the method returns a 400 Bad Request response. On
        /// successful creation, a 201 Created response is returned with the location of the newly created event schedule. If
        /// the creation fails, a 422 Unprocessable Entity response is returned.</remarks>
        /// <param name="request">The request object containing the details required to create a new event schedule. Must not be null and must satisfy
        /// model validation requirements.</param>
        /// <returns>An ActionResult containing the created EventScheduleResult if successful; otherwise, a BadRequest result if the
        /// input is invalid, or an UnprocessableEntity result if the event schedule could not be created.</returns>
        [HttpPost]
        [EndpointName("CreateScheduleAsync")]
        [ProducesResponseType(typeof(EventScheduleResult), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<EventScheduleResult>> CreateScheduleAsync([FromBody] CreateEventScheduleRequest request)
        {
            var result = await scheduleService.CreateEventScheduleAsync(request);

            if (result != null)
            {
                return CreatedAtAction("GetScheduleById", new { scheduleId = result.Id }, result);
            }

            return UnprocessableEntity("Unable to create Schedule");
        }

        /// <summary>
        /// Updates the details of an existing event schedule identified by its unique identifier.
        /// </summary>
        /// <remarks>The method validates the input model state before attempting to update the event schedule. If
        /// the model state is invalid, a BadRequest response is returned. If the update cannot be processed, an
        /// UnprocessableEntity response is returned.</remarks>
        /// <param name="id">The unique identifier of the event schedule to update. Must be a positive long value.</param>
        /// <param name="request">An object containing the updated event schedule details. This parameter is required and cannot be null.</param>
        /// <returns>An IActionResult that indicates the result of the update operation. Returns NoContent if the update is
        /// successful; otherwise, returns BadRequest if the input is invalid or UnprocessableEntity if the update
        /// fails.</returns>
        [HttpPut("{id:long}")]
        [EndpointName("UpdateScheduleAsync")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> UpdateScheduleAsync([FromRoute] long id, [FromBody] UpdateEventScheduleRequest request)
        {
            var result = await scheduleService.UpdateScheduleAsync(id, request);

            if (result)
            {
                return NoContent();
            }

            return UnprocessableEntity("Unable to update Schedule");
        }

        /// <summary>
        /// Deletes the event schedule identified by the specified unique identifier.
        /// </summary>
        /// <remarks>This method does not delete a event schedule if the specified identifier does not match any
        /// existing event schedule.</remarks>
        /// <param name="id">The unique identifier of the event schedule to delete. Must be a valid long integer corresponding to an existing
        /// event schedule.</param>
        /// <returns>An IActionResult that indicates the result of the delete operation. Returns 204 No Content if the deletion
        /// is successful; otherwise, returns 422 Unprocessable Entity with an error message.</returns>
        [HttpDelete("{id:long}")]
        [EndpointName("DeleteScheduleAsync")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> DeleteScheduleAsync([FromRoute] long id)
        {
            var result = await scheduleService.DeleteScheduleAsync(id);

            if (result)
            {
                return NoContent();
            }

            return UnprocessableEntity("Unable to delete Schedule");
        }
    }
}
