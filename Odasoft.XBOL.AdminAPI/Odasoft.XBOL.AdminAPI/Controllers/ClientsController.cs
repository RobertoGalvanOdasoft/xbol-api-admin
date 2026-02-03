using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.Commons.Requests.Filters;
using XBOL.Admin.Core.DTO;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/clients")]
    [ApiController]
    public class ClientsController : Controller
    {
        [HttpPost]
        [EndpointName("GetClientSeasonEventInfoAsync")]
        public async Task<ActionResult<ClientSeasonEvent>> GetClientSeasonEventInfoAsync([FromBody] ClientFilter filter, [FromServices] ClientService clientService)
        {
            ClientSeasonEvent clientSeasonEvent = await clientService.GetClientSeasonEventInfoAsync(filter);
            return Ok(clientSeasonEvent);
        }
    }
}
