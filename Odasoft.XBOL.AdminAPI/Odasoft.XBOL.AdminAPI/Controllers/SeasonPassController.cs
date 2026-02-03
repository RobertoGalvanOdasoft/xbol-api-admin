using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.Commons.Enums;
using Odasoft.XBOL.DTO;
using System.Reflection;
using XBOL.Admin.Core.DTO;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeasonPassController : Controller
    {
        private readonly IStringLocalizerFactory _localizerFactory;

        public SeasonPassController(IStringLocalizerFactory localizerFactory)
        {
            _localizerFactory = localizerFactory;
        }

        /// <summary>
        /// Get SeasonPass Status List (enum)
        /// </summary>
        [HttpGet("status-list")]
        [EndpointName("GetSeasonPassStatusList")]
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
        /// Get SeasonPass Suspended Reason List (enum)
        /// </summary>
        [HttpGet("suspended-reason-list")]
        [EndpointName("GetSeasonPassSuspendedReasonList")]
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
        /// Get SeasonPass Renewal Type List (enum)
        /// </summary>
        [HttpGet("renewal-type-list")]
        [EndpointName("GetSeasonPassRenewalTypeList")]
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
        /// Updated season pass status
        /// </summary>
        [HttpPut("seasonpass/{id:long}")]
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
