using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.DTO.Requests;
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
        /// <returns>An object containing the client credit account info.</returns>
        [HttpGet("client/{clientId:long}")]
        [EndpointName("GetCreditAccountByClientIdAsync")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreditAccountResult))]
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
        /// <returns>An <see cref="ActionResult{T}"/> containing <see langword="true"/> if the credit account was updated successfully.</returns>
        [HttpPut("{creditAccountId:long}")]
        [EndpointName("UpdateCreditAccountByIdAsync")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
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
        /// <returns>An <see cref="ActionResult{T}"/> containing <see langword="true"/> if the credit account was deleted successfully.</returns>
        [HttpDelete("{creditAccountId:long}")]
        [EndpointName("DeleteCreditAccountByIdAsync")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
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
    }
}
