using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.DTO;
using Odasoft.XBOL.DTO.Requests;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/suite-levels")]
    [ApiController]
    public class SuiteLevelsController : ControllerBase
    {
        private readonly SuiteLevelService _suiteLevelService;

        public SuiteLevelsController(SuiteLevelService suiteLevelService)
        {
            _suiteLevelService = suiteLevelService;
        }

        /// <summary>
        /// Retrieves the suite-level catalog as a collection of list items.
        /// </summary>
        /// <returns>An object containing the suite-level catalog. The collection will
        /// be empty if no items are available.</returns>
        [HttpGet("catalog")]
        [EndpointName("GetSuiteLevelCatalogAsync")]
        public async Task<ActionResult<ICollection<ListItem>>> GetSuiteLevelCatalogAsync()
        {
            var result = await _suiteLevelService.GetSuiteLevelCatalogAsync();
            return Ok(result);
        }

        /// <summary>
        /// Creates a new suite level using the specified request data.
        /// </summary>
        /// <remarks>Returns a 400 Bad Request response if the request model is invalid, or a 422
        /// Unprocessable Entity response if the suite level could not be created.</remarks>
        /// <param name="request">The request object containing the details required to create the suite level. Cannot be null.</param>
        /// <returns>An <see cref="ActionResult{T}"/> containing <see langword="true"/> if the suite level was created
        /// successfully; otherwise, an error response indicating the reason for failure.</returns>
        [HttpPost]
        [EndpointName("CreateSuiteLevelAsync")]
        public async Task<ActionResult<bool>> CreateSuiteLevelAsync([FromBody] CreateSuiteLevelRequest request)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var result = await _suiteLevelService.CreateSuiteLevelAsync(request);

            if (result)
            {
                return Ok(result);
            }

            return UnprocessableEntity("Unable to create Suite Level");
        }

        /// <summary>
        /// Updates the suite level with the specified details.
        /// </summary>
        /// <remarks>Returns a 400 Bad Request response if the request model is invalid, or a 422
        /// Unprocessable Entity response if the update operation fails.</remarks>
        /// <param name="request">An object containing the updated suite level information. Must not be null and must satisfy all model
        /// validation requirements.</param>
        /// <returns>An <see cref="ActionResult{T}"/> containing <see langword="true"/> if the suite level was updated
        /// successfully; otherwise, an error response indicating the reason for failure.</returns>
        [HttpPut]
        [EndpointName("UpdateSuiteLevelAsync")]
        public async Task<ActionResult<bool>> UpdateSuiteLevelAsync([FromBody] UpdateSuiteLevelRequest request)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var result = await _suiteLevelService.UpdateSuiteLevelAsync(request);

            if (result)
            {
                return Ok(result);
            }

            return UnprocessableEntity("Unable to update Suite Level");
        }

        /// <summary>
        /// Deletes the suite level with the specified identifier.
        /// </summary>
        /// <param name="suiteLevelId">The unique identifier of the suite level to delete.</param>
        /// <returns>An <see cref="ActionResult{T}"/> containing <see langword="true"/> if the suite level was successfully
        /// deleted; otherwise, an unprocessable entity result.</returns>
        [HttpDelete("{suiteLevelId}")]
        [EndpointName("DeleteSuiteLevelAsync")]
        public async Task<ActionResult<bool>> DeleteSuiteLevelAsync([FromRoute] long suiteLevelId)
        {
            var result = await _suiteLevelService.DeleteSuiteLevelAsync(suiteLevelId);

            if (result)
            {
                return Ok(result);
            }

            return UnprocessableEntity("Unable to delete Suite Level");
        }
    }
}
