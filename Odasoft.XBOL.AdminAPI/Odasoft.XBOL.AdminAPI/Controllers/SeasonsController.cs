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
        /// Get paginated list of seasons with filters.
        /// </summary>
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
        /// Get season by ID.
        /// </summary>
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
        /// Creates a new season.
        /// </summary>
        /// <param name="request">The details of the season to create.</param>
        [HttpPost]
        [EndpointName("CreateSeasonAsync")]
        [ProducesResponseType(typeof(SeasonResult), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<SeasonResult>> CreateSeasonAsync([FromBody] CreateSeasonRequest request)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var result = await seasonService.CreateSeasonAsync(request);

            if (result != null)
            {
                return CreatedAtAction("GetSeasonById", new { id = result.Id }, result);
            }

            return UnprocessableEntity("Unable to create Season");
        }

        /// <summary>
        /// Updates an existing season.
        /// </summary>
        /// <param name="id">The unique identifier of the season to update.</param>
        /// <param name="request">The updated season details.</param>
        [HttpPut("{id:long}")]
        [EndpointName("UpdateSeasonAsync")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> UpdateSeasonAsync([FromRoute] long id, [FromBody] UpdateSeasonRequest request)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var result = await seasonService.UpdateSeasonAsync(id, request);

            if (result)
            {
                return NoContent();
            }

            return UnprocessableEntity("Unable to update Season");
        }

        /// <summary>
        /// Deletes the season with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the season to delete.</param>
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
