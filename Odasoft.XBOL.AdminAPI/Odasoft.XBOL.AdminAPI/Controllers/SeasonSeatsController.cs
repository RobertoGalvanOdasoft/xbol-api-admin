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
        /// <remarks>This method performs an asynchronous operation to fetch seat prices associated with
        /// the provided season ID. The returned list reflects the current pricing for all seats in the specified
        /// season.</remarks>
        /// <param name="seasonId">The unique identifier of the season for which to obtain seat prices. Must be a positive long value.</param>
        /// <returns>An HTTP 200 response containing a list of <see cref="SeatPriceDTO"/> objects representing the seat prices
        /// for the given season. The list will be empty if no seat prices are available.</returns>
        [HttpGet("{seasonId}/seat-prices")]
        [EndpointName("GetSeatPricesBySeasonIdAsync")]
        [ProducesResponseType(typeof(List<SeatPriceDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<SeatPriceDTO>>> GetSeatPricesBySeasonIdAsync([FromRoute] long seasonId)
        {
            List<SeatPriceDTO> result = await seasonSeatsService.GetSeatPricesBySeasonIdAsync(seasonId);

            return Ok(result);
        }
    }
}
