using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.DTO.Results;
using XBOL.Admin.Core.DTO;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/seasons")]
    [ApiController]
    public class SeasonsController(SeasonService seasonService) : Controller
    {
        [HttpGet]
        [EndpointName("GetSeasonSelectorItemsAsync")]
        public async Task<ActionResult<List<SeasonSelectorItem>>> GetSeasonSelectorItemsAsync()
        {
            List<SeasonSelectorItem> result = await seasonService.GetSeasonSelectorItemsAsync();
            return Ok(result);
        }

        [HttpGet("banner/{seasonId}")]
        [EndpointName("GetSeasonBannerAsync")]
        public async Task<ActionResult<SeasonBanner>> GetSeasonBannerAsync([FromRoute] long seasonId)
        {
            SeasonBanner? result = await seasonService.GetSeasonBannerByEventAsync(seasonId);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("event/{seasonId}")]
        [EndpointName("GetLatestEventIdBySeasonAsync")]
        public async Task<ActionResult<long>> GetLatestEventIdBySeasonAsync([FromRoute] long seasonId)
        {
            long? eventId = await seasonService.GetLatestEventIdBySeasonAsync(seasonId);

            if (eventId == null)
            {
                return NotFound();
            }

            return Ok(eventId);
        }

        [HttpGet("season-key/{seasonId}")]
        [EndpointName("GetSeasonKeyAsync")]
        public async Task<ActionResult<SeasonKeyResult>> GetSeasonKeyAsync([FromRoute] long seasonId)
        {
            string? seasonKey = await seasonService.GetSeasonKeyAsync(seasonId);

            if (string.IsNullOrEmpty(seasonKey))
            {
                return NotFound();
            }

            return Ok(new SeasonKeyResult { Value = seasonKey });
        }
    }
}
