using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ITicketingClient _ticketingClient;

        public CategoriesController(ITicketingClient ticketingClient)
        {
            _ticketingClient = ticketingClient;
        }

        [HttpGet]
        public async Task<ActionResult<ICollection<string>>> GetCategories()
        {
            // TODO: Move categories logic to this API from TicketingApi

            var result = await _ticketingClient.CategoriesAsync();

            return Ok(result);
        }
    }
}
