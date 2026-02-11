using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.DTO;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/season-sections")]
    [ApiController]
    public class SeasonSectionController(SeasonSectionService seasonSectionService) : Controller
    {
        /// <summary>
        /// Retrieves the list of zone prices for the specified season.
        /// </summary>
        /// <param name="seasonId">The unique identifier of the season for which to retrieve zone prices.</param>
        [HttpGet("{seasonId}/zone-prices")]
        [EndpointName("GetZonePricesBySeasonIdAsync")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<ZonePriceDTO>))]
        public async Task<ActionResult<List<ZonePriceDTO>>> GetZonePricesBySeasonIdAsync([FromRoute] long seasonId)
        {
            IList<ZonePriceDTO> result = await seasonSectionService.GetZonePricesBySeasonIdAsync(seasonId);

            return Ok(result);
        }
    }
}
