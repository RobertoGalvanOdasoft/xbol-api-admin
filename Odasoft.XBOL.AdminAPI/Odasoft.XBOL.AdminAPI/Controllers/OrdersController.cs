using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.Commons.Requests.Filters;
using Odasoft.XBOL.Commons.Responses;
using Odasoft.XBOL.DTO.QueryParams;
using Odasoft.XBOL.DTO.Requests;
using Odasoft.XBOL.DTO.Results;
using XBOL.Admin.Core.DTO;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ITicketingClient _ticketingClient;
        private readonly OrderService _orderService;

        public OrdersController(ITicketingClient ticketingClient, OrderService orderService)
        {
            _ticketingClient = ticketingClient;
            _orderService = orderService;
        }

        [HttpGet]
        [EndpointName("GetOrdersAsync")]
        public async Task<ActionResult<Odasoft.XBOL.DTO.Response.PagedResponse<OrderResult>>> GetOrdersAsync([FromQuery] OrdersQueryParams queryParams)
        {
            var result = await _orderService.GetOrdersAsync(queryParams);

            return Ok(result);
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
            ClientSeasonEvent result = await _orderService.GetClientSeasonEventByOrderReferenceAsync(orderReference);
            return Ok(result);
        }

        [HttpPost("book-season")]
        [EndpointName("BookSeasonAsync")]
        public async Task<ActionResult<List<string>>> BookSeasonAsync([FromBody] BookSeasonRequest request)
        {
            BookingRequest bookingRequest = new BookingRequest();
            bookingRequest.Seats = request.Seats;
            bookingRequest.HoldToken = request.HoldToken;
            bookingRequest.EventId = request.EventKey;

            var result = await _ticketingClient.BookSeatsAsync(bookingRequest);
            await _orderService.BookSeasonAsync(request);

            return Ok(result);
        }
    }
}
