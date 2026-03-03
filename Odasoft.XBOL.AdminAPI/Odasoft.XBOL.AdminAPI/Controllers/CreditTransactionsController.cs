using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.DTO.QueryParams;
using Odasoft.XBOL.DTO.Requests;
using Odasoft.XBOL.DTO.Response;
using Odasoft.XBOL.DTO.Results;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/credit-transactions")]
    [ApiController]
    public class CreditTransactionsController : ControllerBase
    {
        private readonly ClientCreditTransactionService _clientCreditTransactionService;

        public CreditTransactionsController(ClientCreditTransactionService clientCreditTransactionService)
        {
            _clientCreditTransactionService = clientCreditTransactionService;
        }

        /// <summary>
        /// Retrieves a paged list of credit transactions that match the specified query parameters.
        /// </summary>
        /// <remarks>This method is asynchronous and may take additional time to complete depending on the
        /// number of transactions and the complexity of the query parameters.</remarks>
        /// <param name="queryParams">The parameters used to filter and paginate the credit transactions. This parameter must not be null.</param>
        /// <returns>An ActionResult containing a paged response with the total number of matching transactions and the current page of results.</returns>
        [HttpGet]
        [EndpointName("GetCreditTransactionsAsync")]
        [ProducesResponseType(typeof(PagedResponse<CreditTransactionResult>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<CreditTransactionResult>>> GetCreditTransactionsAsync([FromQuery] CreditTransactionsQueryParams queryParams)
        {
            var result = await _clientCreditTransactionService.GetCreditTransactionsAsync(queryParams);

            return Ok(result);
        }

        /// <summary>
        /// Creates a new credit transaction for the credit account based on the specified request data.
        /// </summary>
        /// <param name="creditAccountId">The unique identifier of the credit account.</param>
        /// <param name="request">An object containing the info for the credit transaction to create.</param>
        /// <returns>An ActionResult containing <see langword="true"/> if the credit transaction was created successfully.</returns>
        [HttpPost("{creditAccountId:long}")]
        [EndpointName("CreateCreditTransactionByCreditAccountIdAsync")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateCreditTransactionByCreditAccountIdAsync([FromRoute] long creditAccountId, [FromBody] ClientCreditTransactionRequest request)
        {
            bool result = await _clientCreditTransactionService.CreateCreditTransactionByCreditAccountIdAsync(creditAccountId, request);

            if (result)
            {
                return Ok(result);
            }

            return BadRequest("Unable to create credit transaction.");
        }

        /// <summary>
        /// Updates an existing credit transaction with the specified details.
        /// </summary>
        /// <param name="creditTransactionId">Unique identifier.</param>
        /// <param name="request">The request object containing the updated credit transaction information.</param>
        /// <returns>An ActionResult containing <see langword="true"/> if the credit transaction was updated successfully.</returns>
        [HttpPut("{creditTransactionId:long}")]
        [EndpointName("UpdateCreditTransactionByIdAsync")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateCreditTransactionByIdAsync([FromRoute] long creditTransactionId, [FromBody] ClientCreditTransactionRequest request)
        {
            bool result = await _clientCreditTransactionService.UpdateCreditTransactionByIdAsync(creditTransactionId, request);

            if (result)
            {
                return Ok(result);
            }

            return NotFound();
        }

        /// <summary>
        /// Deletes an existing credit transaction.
        /// </summary>
        /// <param name="creditTransactionId">Unique identifier</param>
        /// <returns>An ActionResult containing <see langword="true"/> if the credit transaction was deleted successfully.</returns>
        [HttpDelete("{creditTransactionId:long}")]
        [EndpointName("DeleteCreditTransactionByIdAsync")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteCreditTransaction(long creditTransactionId)
        {
            bool result = await _clientCreditTransactionService.DeleteCreditTransactionByIdAsync(creditTransactionId);

            if (result)
            {
                return Ok(result);
            }

            return NotFound();
        }
    }
}
