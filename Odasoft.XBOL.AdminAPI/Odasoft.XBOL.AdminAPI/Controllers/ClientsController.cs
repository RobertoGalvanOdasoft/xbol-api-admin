using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.Commons.Enums;
using Odasoft.XBOL.Commons.Requests.Filters;
using Odasoft.XBOL.DTO;
using Odasoft.XBOL.DTO.QueryParams;
using Odasoft.XBOL.DTO.Requests;
using Odasoft.XBOL.DTO.Response;
using Odasoft.XBOL.DTO.Results;
using System.Globalization;
using System.Reflection;
using System.Text;
using XBOL.Admin.Core.DTO;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/clients")]
    [ApiController]
    public class ClientsController(ClientService clientService, IStringLocalizerFactory _localizerFactory) : ControllerBase
    {
        // TODO: Check this method route, check if it belongs in Season Pass controller
        [HttpPost("season")]
        [EndpointName("GetClientSeasonEventInfoAsync")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ClientSeasonEvent))]
        public async Task<ActionResult<ClientSeasonEvent>> GetClientSeasonEventInfoAsync([FromBody] ClientFilter filter, [FromServices] ClientService clientService)
        {
            ClientSeasonEvent? clientSeasonEvent = await clientService.GetClientSeasonEventInfoAsync(filter);

            if (clientSeasonEvent is null)
            {
                return NotFound();
            }

            return Ok(clientSeasonEvent);
        }

        /// <summary>
        /// Searches for client contacts using the specified phone number or email address.
        /// </summary>
        /// <remarks>If both phone and email are null or empty, the request returns a BadRequest response
        /// indicating that at least one search parameter must be specified.</remarks>
        /// <param name="phone">The phone number to search for. At least one of the parameters, either phone or email, must be provided.</param>
        /// <param name="email">The email address to search for. At least one of the parameters, either phone or email, must be provided.</param>
        /// <returns>An ActionResult containing a ClientContactResponse with the search results. Returns 200 OK if matching
        /// clients are found, 400 Bad Request if neither parameter is provided, or 404 Not Found if no matching clients
        /// exist.</returns>
        [HttpGet("search")]
        [EndpointName("SearchClientAsync")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ClientContactResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClientContactResponse>> SearchClientAsync([FromQuery] string phone, [FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(phone) && string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("At least one search parameter (phone or email) must be provided.");
            }

            ClientContactResponse? result = await clientService.SearchClientAsync(phone, email);

            if (result == null)
            {
                return NotFound("No matching clients found.");
            }

            return Ok(result);
        }

        [HttpGet]
        [EndpointName("GetClientsAsync")]
        public async Task<ActionResult<PagedResponse<ClientResult>>> GetClientsAsync([FromQuery] ClientsQueryParams queryParams)
        {
            var result = await clientService.GetClientsAsync(queryParams);

            return Ok(result);
        }

        [HttpGet("{clientId:long}")]
        [EndpointName("GetClientDetailById")]
        public async Task<ActionResult<ClientDetailResult>> GetClientDetailById([FromRoute] long clientId)
        {
            var result = await clientService.GetClientDetailById(clientId);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost]
        [EndpointName("CreateClientAsync")]
        public async Task<ActionResult<ClientResult>> CreateClientAsync([FromBody] CreateClientRequest request)
        {
            var result = await clientService.CreateClientAsync(request);

            return Ok(result);
        }

        [HttpPut("{id:long}")]
        [EndpointName("UpdateClientAsync")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<ClientResult>> UpdateClientAsync([FromRoute] long id, [FromBody] UpdateClientRequest request)
        {
            var result = await clientService.UpdateClientAsync(id, request);

            if (result)
            {
                return NoContent();
            }

            return UnprocessableEntity("Unable to update Client");
        }

        [HttpDelete("{id:long}")]
        [EndpointName("DeleteClientAsync")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteClientAsync(long id)
        {
            try
            {
                await clientService.DeleteClientAsync(id);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost("download")]
        [EndpointName("DownloadClientsCsvAsync")]
        [Produces("text/csv")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileResult))]
        public async Task<IActionResult> DownloadClientsCsvAsync([FromBody] List<long> clientIds)
        {
            if (clientIds == null || clientIds.Count == 0)
            {
                return BadRequest("No IDs provided.");
            }

            var records = await clientService.GetClientsListByIdAsync(clientIds);

            if (records == null || records.Count == 0)
            {
                return NotFound("No clients found for provided IDs.");
            }

            var memoryStream = new MemoryStream();

            var encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);

            using (var streamWriter = new StreamWriter(memoryStream, encoding, leaveOpen: true))
            using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                csvWriter.Context.RegisterClassMap<ClientCsvMap>();

                await csvWriter.WriteRecordsAsync(records);
                await streamWriter.FlushAsync();
            }

            memoryStream.Position = 0;

            return File(memoryStream, "text/csv", "clients.csv");
        }

        [HttpGet("credit-status-list")]
        [EndpointName("GetCreditStatusList")]
        public ActionResult<List<EnumItemDto>> GetCreditStatusList()
        {
            var enumType = typeof(CreditStatus);

            var localizer = _localizerFactory.Create(
                baseName: enumType.Name,
                location: Assembly.GetExecutingAssembly().GetName().Name!
            );

            var result = Enum.GetValues(enumType)
                .Cast<Enum>()
                .Select(e => new EnumItemDto
                {
                    Value = Convert.ToInt32(e),
                    Label = localizer[e.ToString()]
                })
                .ToList();

            return Ok(result);
        }

        [HttpGet("client-type-list")]
        [EndpointName("GetClientTypeList")]
        public ActionResult<List<EnumItemDto>> GetClientTypeList()
        {
            var enumType = typeof(ClientType);

            var localizer = _localizerFactory.Create(
                baseName: enumType.Name,
                location: Assembly.GetExecutingAssembly().GetName().Name!
            );

            var result = Enum.GetValues(enumType)
                .Cast<Enum>()
                .Select(e => new EnumItemDto
                {
                    Value = Convert.ToInt32(e),
                    Label = localizer[e.ToString()]
                })
                .ToList();

            return Ok(result);
        }

        // TODO: Move to Credit controller
        [HttpGet("payment-frequency-list")]
        [EndpointName("GetPaymentFrequencyList")]
        public ActionResult<List<EnumItemDto>> GetPaymentFrequencyList()
        {
            var enumType = typeof(PaymentFrequency);

            var localizer = _localizerFactory.Create(
                baseName: enumType.Name,
                location: Assembly.GetExecutingAssembly().GetName().Name!
            );

            var result = Enum.GetValues(enumType)
                .Cast<Enum>()
                .Select(e => new EnumItemDto
                {
                    Value = Convert.ToInt32(e),
                    Label = localizer[e.ToString()]
                })
                .ToList();

            return Ok(result);
        }
    }
}
