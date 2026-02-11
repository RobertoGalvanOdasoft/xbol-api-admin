using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.DTO.QueryParams;
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
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}
