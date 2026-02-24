using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/hold-seats")]
    [ApiController]
    public class HoldController(ITicketingClient ticketingClient) : ControllerBase
    {
        // TODO: Do DB actions of creating and releasing hold tokens
        [HttpPost]
        [EndpointName("HoldSeatsAsync")]
        public async Task<ActionResult<HoldToken>> HoldSeatsAsync(HoldSeatsRequest request)
        {
            var token = await ticketingClient.HoldSeatsAsync(request);

            return Ok(token);
        }

        [HttpGet]
        [EndpointName("GetHoldTokenAsync")]
        public async Task<ActionResult<HoldToken>> GetHoldTokenAsync([FromQuery] string holdToken)
        {
            var result = await ticketingClient.GetHoldTokenAsync(holdToken);
            return Ok(result);
        }

        [HttpDelete]
        [EndpointName("ReleaseHoldSeatsAsync")]
        public async Task<ActionResult<HoldToken>> ReleaseHoldSeatsAsync([FromQuery] string holdToken)
        {
            var result = await ticketingClient.ReleaseHoldSeatsAsync(holdToken);
            return Ok(result);
        }
    }
}
