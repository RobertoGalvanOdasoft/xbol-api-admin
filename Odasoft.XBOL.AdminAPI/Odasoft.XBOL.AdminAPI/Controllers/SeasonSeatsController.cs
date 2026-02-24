using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.DTO;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/season-seats")]
    [ApiController]
    public class SeasonSeatsController(SeasonSeatsService seasonSeatsService) : ControllerBase
    {
        /// <summary>
        /// Retrieves the list of seat prices for the specified season.
        /// </summary>
        /// <param name="seasonId">The unique identifier of the season for which to retrieve seat prices.</param>
        [HttpGet("{seasonId}/seat-prices")]
        [EndpointName("GetSeatPricesBySeasonIdAsync")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<SeatPriceDTO>))]
        public async Task<ActionResult<List<SeatPriceDTO>>> GetSeatPricesBySeasonIdAsync([FromRoute] long seasonId)
        {
            IList<SeatPriceDTO> result = await seasonSeatsService.GetSeatPricesBySeasonIdAsync(seasonId);

            return Ok(result);
        }
    }
}
