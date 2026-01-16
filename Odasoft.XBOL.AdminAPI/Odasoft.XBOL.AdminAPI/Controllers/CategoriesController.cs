using Microsoft.AspNetCore.Mvc;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController(ITicketingClient ticketingClient) : ControllerBase
    {
        [HttpGet]
        [EndpointName("GetCategories")]
        public async Task<ActionResult<ICollection<string>>> GetCategories()
        {
            var result = await ticketingClient.GetCategoriesAsync();

            return Ok(result);
        }
    }
}
