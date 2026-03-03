using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.DTO;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/season-sections")]
    [ApiController]
    public class SeasonSectionController(SeasonSectionService seasonSectionService) : ControllerBase
    {
        /// <summary>
        /// Retrieves the collection of zone prices associated with the specified season identifier.
        /// </summary>
        /// <remarks>This method is asynchronous and responds to an HTTP GET request. Ensure that the
        /// provided season identifier exists to avoid receiving an empty result.</remarks>
        /// <param name="seasonId">The unique identifier of the season for which to obtain zone prices. Must be a valid long integer.</param>
        /// <returns>An <see cref="ActionResult{T}"/> containing a list of <see cref="ZonePriceDTO"/> objects that represent the
        /// zone prices for the given season. Returns an empty list if no zone prices are found.</returns>
        [HttpGet("{seasonId:long}/zone-prices")]
        [EndpointName("GetZonePricesBySeasonIdAsync")]
        [ProducesResponseType(typeof(List<ZonePriceDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ZonePriceDTO>>> GetZonePricesBySeasonIdAsync([FromRoute] long seasonId)
        {
            List<ZonePriceDTO> result = await seasonSectionService.GetZonePricesBySeasonIdAsync(seasonId);

            return Ok(result);
        }
    }
}
