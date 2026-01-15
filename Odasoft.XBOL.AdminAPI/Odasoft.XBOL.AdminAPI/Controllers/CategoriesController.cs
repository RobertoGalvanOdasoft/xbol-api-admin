using Microsoft.AspNetCore.Mvc;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController(ITicketingClient ticketingClient) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<ICollection<string>>> GetCategories()
        {
            var result = await ticketingClient.CategoriesAsync();

            return Ok(result);
        }
    }
}
