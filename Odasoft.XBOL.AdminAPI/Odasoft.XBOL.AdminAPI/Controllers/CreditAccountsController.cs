using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.DTO.QueryParams;
using Odasoft.XBOL.DTO.Requests;
using Odasoft.XBOL.DTO.Response;
using Odasoft.XBOL.DTO.Results;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/credit-accounts")]
    [ApiController]
    public class CreditAccountsController : ControllerBase
    {
        private readonly ClientCreditAccountService _creditAccountService;

        public CreditAccountsController(ClientCreditAccountService creditAccountService)
        {
            _creditAccountService = creditAccountService;
        }

        /// <summary>
        /// Retrieves the client credit account information using the client Id as identifier.
        /// </summary>
        /// <param name="clientId">Client unique identifier.</param>
        /// <returns>An ActionResult containing the client credit account info.</returns>
        [HttpGet("client/{clientId:long}")]
        [EndpointName("GetCreditAccountByClientIdAsync")]
        [ProducesResponseType(typeof(CreditAccountResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CreditAccountResult>> GetCreditAccountByClientIdAsync([FromRoute] long clientId)
        {
            CreditAccountResult? result = await _creditAccountService.GetCreditAccountByClientIdAsync(clientId);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Updates an exisiting client credit account with the specified details
        /// </summary>
        /// <param name="creditAccountId">Unique identifier.</param>
        /// <param name="request">An object containing the updated credit account information.</param>
        /// <returns>An actiron result containing <see langword="true"/> if the credit account was updated successfully.</returns>
        [HttpPut("{creditAccountId:long}")]
        [EndpointName("UpdateCreditAccountByIdAsync")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateCreditAccountByIdAsync([FromRoute] long creditAccountId, [FromBody] UpdateClientCreditAccountRequest request)
        {
            bool result = await _creditAccountService.UpdateClientCreditAccountByIdAsync(creditAccountId, request);

            if (result)
            {
                return Ok(result);
            }

            return NotFound();
        }

        /// <summary>
        /// Deletes an existing client credit.
        /// </summary>
        /// <param name="creditAccountId">Unique identifier</param>
        /// <returns>An ActionResult containing <see langword="true"/> if the credit account was deleted successfully.</returns>
        [HttpDelete("{creditAccountId:long}")]
        [EndpointName("DeleteCreditAccountByIdAsync")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteCreditAccountByIdAsync([FromRoute] long creditAccountId)
        {
            bool result = await _creditAccountService.DeleteClientCreditAccountByIdAsync(creditAccountId);

            if (result)
            {
                return Ok(result);
            }

            return NotFound();
        }

        /// <summary>
        /// Retrieves a paginated list of credit orders that match the specified query parameters.
        /// </summary>
        /// <remarks>This method is asynchronous and returns an HTTP 200 response with the paginated
        /// results if successful. Ensure that the provided query parameters are valid to avoid unexpected
        /// results.</remarks>
        /// <param name="queryParams">The parameters used to filter, sort, and paginate the credit orders. This parameter must not be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="ActionResult{T}"/>
        /// whose value is a paged response with the paginated list of credit orders.</returns>
        [HttpGet("orders")]
        [EndpointName("GetCreditOrdersAsync")]
        [ProducesResponseType(typeof(PagedResponse<OrderResult>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<OrderResult>>> GetCreditOrdersAsync([FromQuery] OrdersQueryParams queryParams)
        {
            var result = await _creditAccountService.GetCreditOrdersAsync(queryParams);

            return Ok(result);
        }
    }
}
