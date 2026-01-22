using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController(ITicketingClient ticketingClient) : ControllerBase
    {
        [HttpGet("names")]
        [EndpointName("GetCategoriesNamesAsync")]
        public async Task<ActionResult<ICollection<string>>> GetCategoriesNamesAsync()
        {
            var result = await ticketingClient.GetCategoriesNamesAsync();

            return Ok(result);
        }
    }
}
