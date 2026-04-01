using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.Commons.Requests.Filters;
using Odasoft.XBOL.Commons.Responses;
using Odasoft.XBOL.DTO.Responses;
using XBOL.Admin.Core.DTO;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrdersController(OrderService orderService)
        {
            _orderService = orderService;
        }

        // TODO: Fix logic and implementation
        // TODO: check what part this affects in the FrontEnd
        /// <summary>
        /// Retrieves a paginated list of orders that match the specified filter criteria.
        /// </summary>
        /// <remarks>This method is asynchronous and should be awaited. Ensure that the filter criteria
        /// are valid to avoid unexpected results. The returned list may be empty if no orders match the specified
        /// filters.</remarks>
        /// <param name="filters">An object containing the filter criteria to apply when retrieving the order list, such as date range, order
        /// status, and customer identifier. Cannot be null.</param>
        /// <returns>An asynchronous operation that returns an HTTP 200 response containing a paged result of order list items
        /// matching the provided filters.</returns>
        [HttpPost]
        [EndpointName("GetOrderListAsync")]
        [ProducesResponseType(typeof(PagedResponse<OrderListItem>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<OrderListItem>>> GetOrderListAsync([FromBody] OrderListFilters filters)
        {
            PagedResponse<OrderListItem> result = await _orderService.GetOrderListAsync(filters);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves the season event information associated with the specified order reference for a client.
        /// </summary>
        /// <remarks>This method is intended to provide information about the season pass linked to the
        /// client's order reference. It is recommended to refactor this method to retrieve season pass information
        /// using client ID and season ID to reduce dependency on the order reference.</remarks>
        /// <param name="orderReference">The unique identifier for the order reference used to fetch the corresponding season event information. This
        /// parameter cannot be null or empty.</param>
        /// <returns>An ActionResult containing the ClientSeasonEvent associated with the specified order reference. Returns 200
        /// OK if the event is found; otherwise, an appropriate error response.</returns>
        [HttpGet("{orderReference}")]
        [EndpointName("GetClientSeasonEventByOrderReferenceAsync")]
        [ProducesResponseType(typeof(ClientSeasonEvent), StatusCodes.Status200OK)]
        public async Task<ActionResult<ClientSeasonEvent>> GetClientSeasonEventByOrderReferenceAsync([FromRoute] string orderReference)
        {
            // TODO: This method is being used to get the information of the season pass that the client has in order to show the information in the renewal page,
            // but it should be refactored to get the information of the season pass by the client id and season id, this way we can avoid the dependency with the
            // order reference and we can also get the information of the season pass even if the client doesn't have an order reference for that season pass

            ClientSeasonEvent result = await _orderService.GetClientSeasonEventByOrderReferenceAsync(orderReference);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves the information of the order to be renewed and to display the information that order.
        /// </summary>
        /// <param name="orderReference">The unique reference identifier of the order.</param>
        /// <returns>An ActionResult containing the order details for renovation or display.</returns>
        [HttpGet("renewal-info/{orderReference}")]
        [EndpointName("GetOrderRenawalInfoByReferenceAsync")]
        [ProducesResponseType(typeof(OrderRenewalInfoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderRenewalInfoResponse>> GetOrderRenawalInfoByReferenceAsync([FromRoute] string orderReference)
        {
            OrderRenewalInfoResponse? result = await _orderService.GetOrderRenawalInfoByReferenceAsync(orderReference);

            if (result == null)
            {
                return NotFound($"There is no information for Order '{orderReference}'.");
            }

            return Ok(result);
        }

        /// <summary>
        /// Retrieves detailed information about an order using the specified order reference.
        /// </summary>
        /// <param name="orderReference">The unique reference identifier of the order to retrieve information for. Cannot be null or empty.</param>
        /// <returns>An ActionResult containing the order information if found; otherwise, a 404 Not Found response with an error
        /// message.</returns>
        [HttpGet("info/{orderReference}")]
        [EndpointName("GetOrderInfoByReferenceAsync")]
        [ProducesResponseType(typeof(OrderInfoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderInfoResponse>> GetOrderInfoByReferenceAsync([FromRoute] string orderReference)
        {
            OrderInfoResponse? result = await _orderService.GetOrderInfoByReferenceAsync(orderReference);

            if (result == null)
            {
                return NotFound($"There is no information for Order '{orderReference}'.");
            }

            return Ok(result);
        }

        /// <summary>
        /// Evaluates whether a specific order is eligible for renewal.
        /// </summary>
        /// <param name="orderReference">The unique reference identifier of the order.</param>
        /// <returns>An ActionResult containing order information indicating whether the order can be renewed.</returns>
        [HttpGet("{orderReference}/can-renew")]
        [EndpointName("CanOrderBeRenewedAsync")]
        [ProducesResponseType(typeof(CanRenewOrderResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CanRenewOrderResponse>> CanOrderBeRenewedAsync([FromRoute] string orderReference)
        {
            CanRenewOrderResponse canRenew = await _orderService.CanOrderBeRenewedAsync(orderReference);

            if (canRenew == null || canRenew.OrderId == null || canRenew.OrderId == 0)
            {
                return NotFound($"There is no information for Order '{orderReference}'.");
            }

            return Ok(canRenew);
        }
    }
}
