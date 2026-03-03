using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.Commons.Requests.Filters;
using Odasoft.XBOL.Commons.Responses;
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
    }
}
