using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.DTO.Requests;
using Odasoft.XBOL.DTO.Responses;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/order-actions")]
    [ApiController]
    public class OrderActionsController : ControllerBase
    {
        private readonly OrderActionService _orderActionService;

        public OrderActionsController(OrderActionService orderActionService)
        {
            _orderActionService = orderActionService;
        }

        /// <summary>
        /// Performs the specified action on the order identified by the given order ID.
        /// </summary>
        /// <param name="orderId">The unique identifier of the order on which to perform the action.</param>
        /// <param name="request">An object containing the details of the action to perform on the order. Cannot be null.</param>
        /// <returns>An HTTP 200 response containing the log ID of the performed action if successful; otherwise, an HTTP 422
        /// response with an error message if the action cannot be performed.</returns>
        [HttpPost("{orderId:long}/action")]
        [EndpointName("PerformOrderActionAsync")]
        [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult> PerformOrderActionAsync([FromRoute] long orderId, OrderActionRequest request)
        {
            bool? result = await _orderActionService.PerformOrderActionAsync(orderId, request);

            if (result == null)
            {
                return UnprocessableEntity("Unable to perform the requested action on the order.");
            }

            return Ok(result);
        }

        /// <summary>
        /// Retrieves the list of available actions and logs associated with the specified order.
        /// </summary>
        /// <param name="orderId">The unique identifier of the order for which to retrieve actions and logs.</param>
        /// <returns>An HTTP 200 OK response containing a list of order action responses for the specified order.</returns>
        [HttpGet("{orderId:long}")]
        [EndpointName("GetOrderActionsAsync")]
        [ProducesResponseType(typeof(List<OrderActionResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetOrderActionsAsync([FromRoute] long orderId)
        {
            List<OrderActionResponse> result = await _orderActionService.GetOrderLogsAsync(orderId);

            return Ok(result);
        }
    }
}
