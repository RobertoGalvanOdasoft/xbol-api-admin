using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.Commons.Requests.Filters;
using Odasoft.XBOL.Commons.Responses;
using Odasoft.XBOL.DTO.Requests;
using XBOL.Admin.Core.DTO;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController(ITicketingClient ticketingClient) : ControllerBase
    {
        [HttpPost]
        [EndpointName("GetOrderListAsync")]
        public async Task<ActionResult<PagedResponse<OrderListItem>>> GetOrderListAsync([FromBody] OrderListFilters filters, [FromServices] OrderService orderService)
        {
            PagedResponse<OrderListItem> result = await orderService.GetOrderListAsync(filters);
            return Ok(result);
        }

        [HttpGet("{orderReference}")]
        [EndpointName("GetClientSeasonEventByOrderReferenceAsync")]
        public async Task<ActionResult<ClientSeasonEvent>> GetClientSeasonEventByOrderReferenceAsync([FromRoute] string orderReference, [FromServices] OrderService orderService)
        {
            ClientSeasonEvent result = await orderService.GetClientSeasonEventByOrderReferenceAsync(orderReference);
            return Ok(result);
        }

        [HttpPost("book-season")]
        [EndpointName("BookSeasonAsync")]
        public async Task<ActionResult<List<string>>> BookSeasonAsync([FromBody] BookSeasonRequest request, [FromServices] OrderService orderService)
        {
            BookingRequest bookingRequest = new BookingRequest();
            bookingRequest.Seats = request.Seats;
            bookingRequest.HoldToken = request.HoldToken;
            bookingRequest.EventId = request.EventKey;

            var result = await ticketingClient.BookSeatsAsync(bookingRequest);
            await orderService.BookSeasonAsync(request);

            return Ok(result);
        }
    }
}
