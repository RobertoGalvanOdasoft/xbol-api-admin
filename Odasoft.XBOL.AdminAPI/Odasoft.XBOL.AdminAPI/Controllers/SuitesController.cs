using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.DTO.QueryParams;
using Odasoft.XBOL.DTO.Requests;
using Odasoft.XBOL.DTO.Response;
using Odasoft.XBOL.DTO.Results;

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
        /// Retrieves a list of all available test suites.
        /// </summary>
        /// <returns>An object containing a list of test suite results. Returns an empty
        /// list if no suites are available.</returns>
        [HttpGet]
        [EndpointName("GetSuitesAsync")]
        public async Task<ActionResult<PagedResponse<SuiteResult>>> GetSuitesAsync([FromQuery] SuitesQueryParams queryParams)
        {
            var result = await _suiteService.GetSuitesAsync(queryParams);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves the details of a suite with the specified identifier.
        /// </summary>
        /// <param name="suiteId">The unique identifier of the suite to retrieve.</param>
        /// <returns>An object containing the info if the suite is found; otherwise, a
        /// 404 Not Found response.</returns>
        [HttpGet]
        [Route("{suiteId:long}")]
        [EndpointName("GetSuiteByIdAsync")]
        public async Task<ActionResult<SuiteResult>> GetSuiteByIdAsync([FromRoute] long suiteId)
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
        /// <returns>An <see cref="ActionResult{T}"/> containing <see langword="true"/> if the suite was created successfully;
        /// otherwise, <see langword="false"/>. Returns a 400 Bad Request response if the request data is invalid.</returns>
        [HttpPost]
        [EndpointName("CreateSuiteAsync")]
        public async Task<ActionResult<bool>> CreateSuiteAsync([FromBody] CreateSuiteRequest request)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var result = await _suiteService.CreateSuiteAsync(request);

            if (result)
            {
                return Ok(result);
            }

            return UnprocessableEntity("Unable to create Suite");
        }

        /// <summary>
        /// Updates an existing suite with the specified details.
        /// </summary>
        /// <param name="request">The request object containing the updated suite information. Must not be null and must satisfy all
        /// validation requirements.</param>
        /// <returns>An <see cref="ActionResult{T}"/> containing <see langword="true"/> if the suite was updated successfully;
        /// otherwise, <see langword="false"/>.</returns>
        [HttpPut]
        [EndpointName("UpdateSuiteAsync")]
        public async Task<ActionResult<bool>> UpdateSuiteAsync([FromBody] UpdateSuiteRequest request)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var result = await _suiteService.UpdateSuiteAsync(request);

            if (result)
            {
                return Ok(result);
            }

            return UnprocessableEntity("Unable to update Suite");
        }

        /// <summary>
        /// Deletes the suite with the specified identifier.
        /// </summary>
        /// <param name="suiteId">The unique identifier of the suite to delete.</param>
        /// <returns>An <see cref="ActionResult{T}"/> containing <see langword="true"/> if the suite was successfully deleted;
        /// otherwise, <see langword="false"/>.</returns>
        [HttpDelete("{suiteId:long}")]
        [EndpointName("DeleteSuiteAsync")]
        public async Task<ActionResult<bool>> DeleteSuiteAsync([FromRoute] long suiteId)
        {
            var result = await _suiteService.DeleteSuiteAsync(suiteId);

            if (result)
            {
                return Ok(result);
            }

            return UnprocessableEntity("Unable to delete Suite");
        }
    }
}
