using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/hold-seats")]
    [ApiController]
    [Obsolete("Use POST /api/seat-management/{externalKey}/hold")]
    public class HoldController(ITicketingClient ticketingClient) : ControllerBase
    {
        /// <summary>
        /// Initiates a request to hold the specified seats for an event and returns a token representing the seat hold.
        /// </summary>
        /// <remarks>This method is asynchronous and may involve network communication with the ticketing
        /// service. Ensure that the request contains valid seat information to avoid errors.</remarks>
        /// <param name="request">An object containing details about the seats to be held, including event and seat identifiers. Must not be
        /// null.</param>
        /// <returns>An ActionResult containing a hold token that can be used to complete the purchase of the reserved seats.</returns>
        [HttpPost]
        [EndpointName("HoldSeatsAsync")]
        [ProducesResponseType(typeof(HoldToken), StatusCodes.Status200OK)]
        public async Task<ActionResult<HoldToken>> HoldSeatsAsync(HoldSeatsRequest request)
        {
            var token = await ticketingClient.HoldSeatsAsync(request);

            return Ok(token);
        }

        /// <summary>
        /// Retrieves a hold token associated with the specified token string.
        /// </summary>
        /// <remarks>This method asynchronously obtains a hold token from the ticketing client. Ensure
        /// that the holdToken parameter is valid to prevent errors.</remarks>
        /// <param name="holdToken">The hold token string used to identify the specific hold request. This parameter cannot be null or empty.</param>
        /// <returns>An ActionResult containing a HoldToken object that represents the retrieved hold token.</returns>
        [HttpGet]
        [EndpointName("GetHoldTokenAsync")]
        [ProducesResponseType(typeof(HoldToken), StatusCodes.Status200OK)]
        public async Task<ActionResult<HoldToken>> GetHoldTokenAsync([FromQuery] string holdToken)
        {
            var result = await ticketingClient.GetHoldTokenAsync(holdToken);
            return Ok(result);
        }

        /// <summary>
        /// Releases the seats held by the specified hold token.
        /// </summary>
        /// <remarks>This asynchronous operation releases all seats associated with the provided hold
        /// token. Ensure that the hold token is valid and corresponds to an active hold before calling this
        /// method.</remarks>
        /// <param name="holdToken">The token that identifies the held seats to be released. This value cannot be null or empty.</param>
        /// <returns>An ActionResult containing a HoldToken object that represents the outcome of the release operation.</returns>
        [HttpDelete]
        [EndpointName("ReleaseHoldSeatsAsync")]
        [ProducesResponseType(typeof(HoldToken), StatusCodes.Status200OK)]
        public async Task<ActionResult<HoldToken>> ReleaseHoldSeatsAsync([FromQuery] string holdToken)
        {
            var result = await ticketingClient.ReleaseHoldSeatsAsync(holdToken);
            return Ok(result);
        }
    }
}
