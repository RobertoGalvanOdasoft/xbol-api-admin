using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.DTO.QueryParams;
using Odasoft.XBOL.DTO.Requests;
using Odasoft.XBOL.DTO.Response;
using Odasoft.XBOL.DTO.Results;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/seasons")]
    [ApiController]
    public class SeasonsController(SeasonService seasonService) : ControllerBase
    {
        /// <summary>
        /// Retrieves a paginated list of seasons that match the specified query parameters.
        /// </summary>
        /// <remarks>This method is asynchronous and may take additional time to complete depending on the
        /// data source and query complexity. Ensure that the provided query parameters are valid to avoid unexpected
        /// results.</remarks>
        /// <param name="queryParams">The parameters used to filter, sort, and paginate the list of seasons. This parameter must not be null.</param>
        /// <returns>An HTTP 200 response containing a paged result set of season items. The response includes pagination
        /// metadata such as total count and page information.</returns>
        [HttpGet]
        [EndpointName("GetSeasonsAsync")]
        [ProducesResponseType(typeof(PagedResponse<SeasonListItem>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<SeasonListItem>>> GetSeasonsAsync(
            [FromQuery] SeasonsQueryParams queryParams)
        {
            var result = await seasonService.GetSeasonsAsync(queryParams);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves the details of a season specified by its unique identifier.
        /// </summary>
        /// <remarks>This method performs an asynchronous lookup for the season using the provided
        /// identifier. If no season exists for the given identifier, the response will indicate that the resource was
        /// not found.</remarks>
        /// <param name="id">The unique identifier of the season to retrieve. Must be a positive long value.</param>
        /// <returns>An ActionResult containing the season details if found; otherwise, a 404 Not Found response.</returns>
        [HttpGet("{id:long}")]
        [EndpointName("GetSeasonByIdAsync")]
        [ProducesResponseType(typeof(SeasonResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SeasonResult>> GetSeasonByIdAsync([FromRoute] long id)
        {
            var result = await seasonService.GetSeasonByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Creates a new season using the specified request data and returns the result of the operation.
        /// </summary>
        /// <remarks>If the model state is invalid, the method returns a 400 Bad Request response. On
        /// successful creation, a 201 Created response is returned with the location of the newly created season. If
        /// the creation fails, a 422 Unprocessable Entity response is returned.</remarks>
        /// <param name="request">The request object containing the details required to create a new season. Must not be null and must satisfy
        /// model validation requirements.</param>
        /// <returns>An ActionResult containing the created SeasonResult if successful; otherwise, a BadRequest result if the
        /// input is invalid, or an UnprocessableEntity result if the season could not be created.</returns>
        [HttpPost]
        [EndpointName("CreateSeasonAsync")]
        [ProducesResponseType(typeof(SeasonResult), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<SeasonResult>> CreateSeasonAsync([FromBody] CreateSeasonRequest request)
        {
            var result = await seasonService.CreateSeasonAsync(request);

            if (result != null)
            {
                return CreatedAtAction("GetSeasonById", new { id = result.Id }, result);
            }

            return UnprocessableEntity("Unable to create Season");
        }

        /// <summary>
        /// Updates the details of an existing season identified by its unique identifier.
        /// </summary>
        /// <remarks>The method validates the input model state before attempting to update the season. If
        /// the model state is invalid, a BadRequest response is returned. If the update cannot be processed, an
        /// UnprocessableEntity response is returned.</remarks>
        /// <param name="id">The unique identifier of the season to update. Must be a positive long value.</param>
        /// <param name="request">An object containing the updated season details. This parameter is required and cannot be null.</param>
        /// <returns>An IActionResult that indicates the result of the update operation. Returns NoContent if the update is
        /// successful; otherwise, returns BadRequest if the input is invalid or UnprocessableEntity if the update
        /// fails.</returns>
        [HttpPut("{id:long}")]
        [EndpointName("UpdateSeasonAsync")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> UpdateSeasonAsync([FromRoute] long id, [FromBody] UpdateSeasonRequest request)
        {
            var result = await seasonService.UpdateSeasonAsync(id, request);

            if (result)
            {
                return NoContent();
            }

            return UnprocessableEntity("Unable to update Season");
        }

        /// <summary>
        /// Deletes the season identified by the specified unique identifier.
        /// </summary>
        /// <remarks>This method does not delete a season if the specified identifier does not match any
        /// existing season.</remarks>
        /// <param name="id">The unique identifier of the season to delete. Must be a valid long integer corresponding to an existing
        /// season.</param>
        /// <returns>An IActionResult that indicates the result of the delete operation. Returns 204 No Content if the deletion
        /// is successful; otherwise, returns 422 Unprocessable Entity with an error message.</returns>
        [HttpDelete("{id:long}")]
        [EndpointName("DeleteSeasonAsync")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> DeleteSeasonAsync([FromRoute] long id)
        {
            var result = await seasonService.DeleteSeasonAsync(id);

            if (result)
            {
                return NoContent();
            }

            return UnprocessableEntity("Unable to delete Season");
        }
    }
}
