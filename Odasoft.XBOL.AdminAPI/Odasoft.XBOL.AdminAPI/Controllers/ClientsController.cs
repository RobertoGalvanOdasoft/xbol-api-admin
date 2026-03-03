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
        [ProducesResponseType(typeof(ClientSeasonEvent), StatusCodes.Status200OK)]
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
        [ProducesResponseType(typeof(ClientContactResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Asynchronously retrieves a paginated list of clients that match the specified query parameters.
        /// </summary>
        /// <remarks>Use this method to obtain a subset of clients based on filtering and pagination
        /// options. Ensure that the query parameters are set appropriately to retrieve the desired results.</remarks>
        /// <param name="queryParams">The parameters used to filter, sort, and paginate the list of clients. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an object with a paged response of client
        /// objects that match the query criteria.</returns>
        [HttpGet]
        [EndpointName("GetClientsAsync")]
        [ProducesResponseType(typeof(PagedResponse<ClientResult>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<ClientResult>>> GetClientsAsync([FromQuery] ClientsQueryParams queryParams)
        {
            var result = await clientService.GetClientsAsync(queryParams);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves the details of a client specified by the unique client identifier.
        /// </summary>
        /// <remarks>This method asynchronously obtains client details from the client service. Ensure
        /// that the provided client ID is valid and exists in the system.</remarks>
        /// <param name="clientId">The unique identifier of the client to retrieve. Must be a positive long value.</param>
        /// <returns>An object containing the client details if found; otherwise, a 404 Not Found response
        /// if no client exists with the specified identifier.</returns>
        [HttpGet("{clientId:long}")]
        [EndpointName("GetClientDetailById")]
        [ProducesResponseType(typeof(ClientDetailResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClientDetailResult>> GetClientDetailById([FromRoute] long clientId)
        {
            var result = await clientService.GetClientDetailByIdAsync(clientId);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Creates a new client using the specified request data.
        /// </summary>
        /// <remarks>This method is asynchronous and processes the client creation request using the
        /// injected client service. Ensure that the request data is valid to avoid errors during processing.</remarks>
        /// <param name="request">The request object containing the information required to create a new client. This parameter must not be
        /// null.</param>
        /// <returns>An ActionResult containing the result of the client creation operation, including details of the newly
        /// created client.</returns>
        [HttpPost]
        [EndpointName("CreateClientAsync")]
        [ProducesResponseType(typeof(ClientResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<ClientResult>> CreateClientAsync([FromBody] CreateClientRequest request)
        {
            var result = await clientService.CreateClientAsync(request);

            return Ok(result);
        }

        /// <summary>
        /// Updates the client information for the specified client identifier with the provided data.
        /// </summary>
        /// <remarks>This method performs an asynchronous update operation. If the update fails, an error
        /// message is returned indicating that the client could not be updated.</remarks>
        /// <param name="id">The unique identifier of the client to update. Must be a valid long integer.</param>
        /// <param name="request">An object containing the updated client information. Cannot be null.</param>
        /// <returns>An ActionResult that indicates the result of the update operation. Returns 204 No Content if the update is
        /// successful; otherwise, returns 422 Unprocessable Entity if the update cannot be performed.</returns>
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

        /// <summary>
        /// Deletes the client with the specified identifier.
        /// </summary>
        /// <remarks>This method is asynchronous. If the specified client does not exist, a 404 Not Found
        /// response is returned.</remarks>
        /// <param name="id">The unique identifier of the client to delete. Must be a positive long value.</param>
        /// <returns>An response that indicates the result of the operation.
        /// Returns NoContent if the client was successfully deleted.
        /// Returns NotFound if the client does not exist.</returns>
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

        /// <summary>
        /// Generates and returns a CSV file containing client information for the specified client IDs.
        /// </summary>
        /// <remarks>Use this endpoint to export client records as a CSV file for the provided client IDs.
        /// The response includes a UTF-8 encoded CSV file named 'clients.csv'. Ensure that the client IDs correspond to
        /// existing clients to receive data.</remarks>
        /// <param name="clientIds">A list of client IDs for which to retrieve and export client data. Cannot be null or empty.</param>
        /// <returns>A file result containing the CSV file with client data if clients are found; otherwise, a 400 Bad Request if
        /// no IDs are provided, or a 404 Not Found if no matching clients exist.</returns>
        [HttpPost("download")]
        [EndpointName("DownloadClientsCsvAsync")]
        [Produces("text/csv")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Retrieves a list of available credit status options, each represented by an integer value and a localized
        /// label.
        /// </summary>
        /// <remarks>The returned labels are localized according to the current culture, enabling
        /// appropriate display in different languages. The list is generated from the <see cref="CreditStatus"/>
        /// enumeration, ensuring that all defined statuses are included.</remarks>
        /// <returns>A list of objects, where each object contains the integer value and the localized
        /// label of a credit status.</returns>
        [HttpGet("credit-status-list")]
        [EndpointName("GetCreditStatusList")]
        [ProducesResponseType(typeof(List<EnumItemDto>), StatusCodes.Status200OK)]
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

        /// <summary>
        /// Retrieves a list of available client types, each represented by an integer value and a localized label.
        /// </summary>
        /// <remarks>The labels for each client type are localized based on the current culture using the
        /// application's localization resources. This method is intended to provide user-friendly, culture-aware
        /// options for client type selection in user interfaces.</remarks>
        /// <returns>An object containing a list of objects. Each object includes the integer value of a client
        /// type and its corresponding localized label. The list is empty if no client types are defined.</returns>
        [HttpGet("client-type-list")]
        [EndpointName("GetClientTypeList")]
        [ProducesResponseType(typeof(List<EnumItemDto>), StatusCodes.Status200OK)]
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
        /// <summary>
        /// Retrieves a list of available payment frequency options, each represented by an integer value and a
        /// localized label.
        /// </summary>
        /// <remarks>The labels for each payment frequency are localized according to the current culture,
        /// ensuring that the returned text is appropriate for the user's language settings. The list is generated from
        /// the PaymentFrequency enumeration.</remarks>
        /// <returns>A list of where each item contains the integer value and the
        /// corresponding localized label for a payment frequency.</returns>
        [HttpGet("payment-frequency-list")]
        [EndpointName("GetPaymentFrequencyList")]
        [ProducesResponseType(typeof(List<EnumItemDto>), StatusCodes.Status200OK)]
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
