using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.Commons.Enums;
using Odasoft.XBOL.DTO;
using System.Reflection;
using XBOL.Admin.Core.DTO;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/season-pass")]
    [ApiController]
    public class SeasonPassController : ControllerBase
    {
        private readonly IStringLocalizerFactory _localizerFactory;

        public SeasonPassController(IStringLocalizerFactory localizerFactory)
        {
            _localizerFactory = localizerFactory;
        }

        /// <summary>
        /// Retrieves a list of available season pass statuses, each represented by its enumeration value and a
        /// localized label.
        /// </summary>
        /// <remarks>The labels for each status are localized according to the current culture. Ensure
        /// that localization resources for season pass statuses are properly configured to provide accurate
        /// labels.</remarks>
        /// <returns>An ActionResult containing a list of objects, where each object
        /// includes the integer value and localized label of a season pass status.</returns>
        [HttpGet("status-list")]
        [EndpointName("GetSeasonPassStatusList")]
        [ProducesResponseType(typeof(List<EnumItemDto>), StatusCodes.Status200OK)]
        public ActionResult<List<EnumItemDto>> GetSeasonPassStatusList()
        {
            var enumType = typeof(SeasonPassStatus);

            var localizer = _localizerFactory.Create(
                baseName: enumType.Name,
                location: Assembly.GetExecutingAssembly().GetName().Name!
            );

            var result = Enum.GetValues(enumType)
                .Cast<Enum>()
                .Select(e => new EnumItemDto
                {
                    Value = Convert.ToInt32(e),
                    Label = localizer[e.ToString()]
                })
                .ToList();

            return Ok(result);
        }

        /// <summary>
        /// Retrieves a list of reasons for which a season pass may be suspended.
        /// </summary>
        /// <remarks>The method uses the SeasonPassSuspendedReason enumeration to generate
        /// the list of reasons. The labels are localized based on the current culture.</remarks>
        /// <returns>A list of objects representing the suspension reasons, where each object contains
        /// a value and a localized label.</returns>
        [HttpGet("suspended-reason-list")]
        [EndpointName("GetSeasonPassSuspendedReasonList")]
        [ProducesResponseType(typeof(List<EnumItemDto>), StatusCodes.Status200OK)]
        public ActionResult<List<EnumItemDto>> GetSeasonPassSuspendedReasonList()
        {
            var enumType = typeof(SeasonPassSuspendedReason);

            var localizer = _localizerFactory.Create(
                baseName: enumType.Name,
                location: Assembly.GetExecutingAssembly().GetName().Name!
            );

            var result = Enum.GetValues(enumType)
                .Cast<Enum>()
                .Select(e => new EnumItemDto
                {
                    Value = Convert.ToInt32(e),
                    Label = localizer[e.ToString()]
                })
                .ToList();

            return Ok(result);
        }

        /// <summary>
        /// Retrieves a list of available season pass renewal types, each represented by an enumeration item.
        /// </summary>
        /// <remarks>This method uses localization to provide user-friendly labels for each renewal type
        /// based on the current culture.</remarks>
        /// <returns>A list of objects, where each object contains the integer value and localized
        /// label of a season pass renewal type.</returns>
        [HttpGet("renewal-type-list")]
        [EndpointName("GetSeasonPassRenewalTypeList")]
        [ProducesResponseType(typeof(List<EnumItemDto>), StatusCodes.Status200OK)]
        public ActionResult<List<EnumItemDto>> GetSeasonPassRenewalTypeList()
        {
            var enumType = typeof(SeasonPassRenewalType);

            var localizer = _localizerFactory.Create(
                baseName: enumType.Name,
                location: Assembly.GetExecutingAssembly().GetName().Name!
            );

            var result = Enum.GetValues(enumType)
                .Cast<Enum>()
                .Select(e => new EnumItemDto
                {
                    Value = Convert.ToInt32(e),
                    Label = localizer[e.ToString()]
                })
                .ToList();

            return Ok(result);
        }

        /// <summary>
        /// Updates the status of a season pass identified by the specified ID.
        /// </summary>
        /// <remarks>If the provided ID does not match the ID in the request body, a 400 Bad Request
        /// response is returned. If the season pass is not found, a 404 Not Found response is returned.</remarks>
        /// <param name="id">The unique identifier of the season pass to update. This value must match the ID provided in the request
        /// body.</param>
        /// <param name="request">An object containing the new status information for the season pass. This object must include a valid
        /// SeasonPassId that corresponds to the ID in the route.</param>
        /// <param name="seasonPassService">An instance of the SeasonPassService used to perform the update operation on the season pass status.</param>
        /// <returns>An IActionResult indicating the result of the update operation. Returns 204 No Content if the update is
        /// successful.</returns>
        [HttpPut("{id:long}")]
        [EndpointName("UpdateSeasonPassStatusAsync")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateSeasonPassStatusAsync([FromRoute] long id, [FromBody] SeasonPassStatusRequest request, [FromServices] SeasonPassService seasonPassService)
        {
            if (id != request.SeasonPassId)
            {
                return BadRequest("The route ID does not match the ID in the request body.");
            }

            await seasonPassService.UpdateAsync(request);

            return NoContent();
        }
    }
}
