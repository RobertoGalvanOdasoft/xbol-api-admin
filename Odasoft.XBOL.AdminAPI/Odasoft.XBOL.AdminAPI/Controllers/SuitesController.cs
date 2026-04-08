using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.DTO;
using Odasoft.XBOL.DTO.QueryParams;
using Odasoft.XBOL.DTO.Requests;
using Odasoft.XBOL.DTO.Response;
using Odasoft.XBOL.DTO.Responses;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/suites")]
    [ApiController]
    public class SuitesController : ControllerBase
    {
        private readonly SuiteService _suiteService;

        public SuitesController(SuiteService suiteService)
        {
            _suiteService = suiteService;
        }

        /// <summary>
        /// Retrieves a list of all available suites.
        /// </summary>
        /// <returns>An ActionResult containing a list of test suite results. Returns an empty
        /// list if no suites are available.</returns>
        [HttpGet]
        [EndpointName("GetSuitesAsync")]
        [ProducesResponseType(typeof(PagedResponse<SuiteResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<SuiteResponse>>> GetSuitesAsync([FromQuery] SuitesQueryParams queryParams)
        {
            var result = await _suiteService.GetSuitesAsync(queryParams);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves the details of a suite with the specified identifier.
        /// </summary>
        /// <param name="suiteId">The unique identifier of the suite to retrieve.</param>
        /// <returns>An ActionResult containing the info if the suite is found; otherwise, a
        /// 404 Not Found response.</returns>
        [HttpGet]
        [Route("{suiteId:long}")]
        [EndpointName("GetSuiteByIdAsync")]
        [ProducesResponseType(typeof(SuiteResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SuiteResponse>> GetSuiteByIdAsync([FromRoute] long suiteId)
        {
            var result = await _suiteService.GetSuiteByIdAsync(suiteId);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Creates a new suite based on the specified request data.
        /// </summary>
        /// <param name="request">The details of the suite to create. Must not be null and must satisfy all model validation requirements.</param>
        /// <returns>An ActionResult containing <see langword="true"/> if the suite was created successfully;
        /// otherwise, <see langword="false"/>. Returns a 400 Bad Request response if the request data is invalid.</returns>
        [HttpPost]
        [EndpointName("CreateSuiteAsync")]
        [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<long>> CreateSuiteAsync([FromBody] SuiteRequest request)
        {
            var newId = await _suiteService.CreateSuiteAsync(request);

            if (newId > 0)
            {
                return CreatedAtAction("GetSuiteById", new { suiteId = newId }, newId);
            }

            return UnprocessableEntity("Unable to create Suite");
        }

        /// <summary>
        /// Updates an existing suite with the specified details.
        /// </summary>
        /// <param name="suiteId">The unique Id of the Suite.</param>
        /// <param name="request">The request object containing the updated suite information. Must not be null and must satisfy all
        /// validation requirements.</param>
        /// <returns>An ActionResult containing <see langword="true"/> if the suite was updated successfully;
        /// otherwise, <see langword="false"/>.</returns>
        [HttpPut("{suiteId:long}")]
        [EndpointName("UpdateSuiteAsync")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult> UpdateSuiteAsync([FromRoute] long suiteId, [FromBody] SuiteRequest request)
        {
            var result = await _suiteService.UpdateSuiteAsync(suiteId, request);

            if (result)
            {
                return NoContent();
            }

            return UnprocessableEntity("Unable to update Suite");
        }

        /// <summary>
        /// Deletes the suite with the specified identifier.
        /// </summary>
        /// <param name="suiteId">The unique identifier of the suite to delete.</param>
        /// <returns>An ActionResult containing <see langword="true"/> if the suite was successfully deleted;
        /// otherwise, <see langword="false"/>.</returns>
        [HttpDelete("{suiteId:long}")]
        [EndpointName("DeleteSuiteAsync")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult> DeleteSuiteAsync([FromRoute] long suiteId)
        {
            var result = await _suiteService.DeleteSuiteAsync(suiteId);

            if (result)
            {
                return NoContent();
            }

            return UnprocessableEntity("Unable to delete Suite");
        }

        /// <summary>
        /// Retrieves the catalog of suite items associated with the specified suite level.
        /// </summary>
        /// <param name="suiteLevelId">The unique identifier of the suite level for which to retrieve the catalog.</param>
        /// <returns>An asynchronous operation that returns a list containing the
        /// collection of suite catalog items. Returns an empty collection if no items are found for the specified suite
        /// level.</returns>
        [HttpGet("{suiteLevelId:long}/catalog")]
        [EndpointName("GetSuiteCatalogBySuiteLevelIdAsync")]
        [ProducesResponseType(typeof(List<ListItem>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ListItem>>> GetSuiteCatalogBySuiteLevelIdAsync([FromRoute] long suiteLevelId)
        {
            var result = await _suiteService.GetSuiteCatalogBySuiteLevelIdAsync(suiteLevelId);
            return Ok(result);
        }
    }
}
