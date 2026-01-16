using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController(ITicketingClient ticketingClient) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<ICollection<string>>> GetCategoriesAsync()
        {
            var result = await ticketingClient.GetCategoriesAsync();

            return Ok(result);
        }
    }
}
