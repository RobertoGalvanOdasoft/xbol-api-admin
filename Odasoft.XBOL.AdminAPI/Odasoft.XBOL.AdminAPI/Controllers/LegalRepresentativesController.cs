using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.DTO.Requests;
using Odasoft.XBOL.DTO.Results;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/legal-representatives")]
    [ApiController]
    public class LegalRepresentativesController : ControllerBase
    {
        private readonly LegalRepresentativeService _legalRepresentativeService;

        public LegalRepresentativesController(LegalRepresentativeService legalRepresentativeService)
        {
            _legalRepresentativeService = legalRepresentativeService;
        }

        /// <summary>
        /// Retreives the detail of a legal representative using the client Id as the identifier.
        /// </summary>
        /// <param name="clientId">The unique identifier of the client.</param>
        /// <returns>An ActionResult containing the info of the legal representative. If is not found; otherwise,
        /// a 404 Not Found response.</returns>
        [HttpGet("{clientId:long}")]
        [EndpointName("GetLegalRepresentativeByClientIdAsync")]
        [ProducesResponseType(typeof(LegalRepresentativeResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetLegalRepresentativeByClientIdAsync([FromRoute] long clientId)
        {
            LegalRepresentativeResult? result = await _legalRepresentativeService.GetLegalRepresentativeByClientIdAsync(clientId);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Creates a new legal representative based on the specified request data.
        /// </summary>
        /// <param name="request">The details of the legal representative to create.</param>
        /// <returns>An ActionResult containing <see langword="true"/> if the suite was created successfully.</returns>
        [HttpPost]
        [EndpointName("CreateLegalRepresentativeAsync")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateLegalRepresentativeAsync([FromBody] LegalRepresentativeRequest request)
        {
            bool result = await _legalRepresentativeService.CreateLegalRepresentativeAsync(request);

            if (result)
            {
                return Ok(result);
            }

            return BadRequest("Unable to create legal representative.");
        }

        /// <summary>
        /// Updates an existing legal representative with the specified details, using the Client Id as unique identifier.
        /// </summary>
        /// <param name="request">The request object containing the updated legal representative information.</param>
        /// <returns>An ActionResult containing <see langword="true"/> if the suite was updated successfully.</returns>
        [HttpPut]
        [EndpointName("UpdateLegalRepresentativeByClientIdAsync")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateLegalRepresentativeByClientIdAsync([FromBody] LegalRepresentativeRequest request)
        {
            bool result = await _legalRepresentativeService.UpdateLegalRepresentativeByClientIdAsync(request);

            if (result)
            {
                return Ok(result);
            }

            return NotFound();
        }

        /// <summary>
        /// Deletes the legal representative with the specified client as identifier.
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns>An ActionResult containing <see langword="true"/> if the suite was deleted successfully.</returns>
        [HttpDelete("{clientId:long}")]
        [EndpointName("DeleteLegalRepresentativeByClientIdAsync")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteLegalRepresentativeByClientIdAsync([FromRoute] long clientId)
        {
            bool result = await _legalRepresentativeService.DeleteLegalRepresentativeByClientIdAsync(clientId);

            if (result)
            {
                return Ok(result);
            }

            return NotFound();
        }
    }
}
