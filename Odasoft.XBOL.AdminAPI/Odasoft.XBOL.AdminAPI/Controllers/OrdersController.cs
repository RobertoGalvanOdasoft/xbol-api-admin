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
        [HttpPost]
        [EndpointName("GetOrderListAsync")]
        public async Task<ActionResult<PagedResponse<OrderListItem>>> GetOrderListAsync([FromBody] OrderListFilters filters)
        {
            PagedResponse<OrderListItem> result = await _orderService.GetOrderListAsync(filters);
            return Ok(result);
        }

        [HttpGet("{orderReference}")]
        [EndpointName("GetClientSeasonEventByOrderReferenceAsync")]
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
