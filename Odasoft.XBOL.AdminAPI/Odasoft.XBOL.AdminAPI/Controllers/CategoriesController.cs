using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ITicketingApi _ticketingApi;

        public CategoriesController(ITicketingApi ticketingApi)
        {
            _ticketingApi = ticketingApi;
        }

        [HttpGet]
        public async Task<ActionResult<ICollection<string>>> GetCategories()
        {
            // TODO: Move categories logic to this API from TicketingApi

            var result = await _ticketingApi.CategoriesAsync();

            return Ok(result);
        }
    }
}
