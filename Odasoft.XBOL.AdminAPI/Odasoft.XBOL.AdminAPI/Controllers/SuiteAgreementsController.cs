using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.AdminAPI.Customs;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.DTO.Requests;
using Odasoft.XBOL.DTO.Results;
using System.IO.Compression;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/suite-agreements")]
    [ApiController]
    public class SuiteAgreementsController : ControllerBase
    {
        private readonly SuiteAgreementService _suiteAgreementService;

        public SuiteAgreementsController(SuiteAgreementService suiteAgreementService)
        {
            _suiteAgreementService = suiteAgreementService;
        }

        /// <summary>
        /// Retrieves a list of suite agreements.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of objects
        /// representing the suite agreements.</returns>
        [HttpGet]
        [EndpointName("GetSuiteAgreementsAsync")]
        public async Task<ActionResult<List<SuiteAgreementResult>>> GetSuiteAgreementsAsync()
        {
            var suiteAgreements = await _suiteAgreementService.GetSuiteAgreementsAsync();
            return Ok(suiteAgreements);
        }

        /// <summary>
        /// Retrieves the suite agreement with the specified identifier.
        /// </summary>
        /// <param name="suiteAgreementId">The unique identifier of the suite agreement to retrieve.</param>
        /// <returns>An object containing the suite agreement if found; otherwise, a 404 Not Found
        /// response.</returns>
        [HttpGet("{suiteAgreementId:long}")]
        [EndpointName("GetSuiteAgreementByIdAsync")]
        public async Task<ActionResult<SuiteAgreementResult>> GetSuiteAgreementByIdAsync([FromRoute] long suiteAgreementId)
        {
            var suiteAgreement = await _suiteAgreementService.GetSuiteAgreementByIdAsync(suiteAgreementId);

            if (suiteAgreement == null)
            {
                return NotFound();
            }

            return Ok(suiteAgreement);
        }

        /// <summary>
        /// Creates a new Suite Agreement using the specified request data.
        /// </summary>
        /// <param name="request">The details of the Suite Agreement to create. Cannot be null.</param>
        /// <returns>A 201 Created result if the Suite Agreement is successfully created; otherwise, a 422 Unprocessable Entity
        /// result with an error message.</returns>
        [HttpPost]
        [EndpointName("CreateSuiteAgreementAsync")]
        public async Task<ActionResult> CreateSuiteAgreementAsync([FromForm] CreateSuiteAgreementRequest request)
        {
            var success = await _suiteAgreementService.CreateSuiteAgreementAsync(request);

            if (success)
            {
                return Ok(success);
            }

            // TODO: Return a server error
            return UnprocessableEntity("Unable to create the Suite Agreement");
        }

        /// <summary>
        /// Updates an existing suite agreement with the specified details.
        /// </summary>
        /// <param name="request">The request object containing the updated suite agreement information. Must not be null.</param>
        /// <returns>An <see cref="OkResult"/> if the update is successful; a <see cref="NotFoundObjectResult"/> if the suite
        /// agreement is not found; or an <see cref="UnprocessableEntityObjectResult"/> if the update cannot be
        /// processed.</returns>
        [HttpPut]
        [EndpointName("UpdateSuiteAgreementAsync")]
        public async Task<ActionResult> UpdateSuiteAgreementAsync([FromBody] UpdateSuiteAgreementRequest request)
        {
            try
            {
                var success = await _suiteAgreementService.UpdateSuiteAgreementAsync(request);

                if (success)
                {
                    return Ok();
                }

                return NotFound($"Suite agreement with Id '{request.Id}' not found ");
            }
            catch (Exception)
            {
                // TODO: Return a server error
                return UnprocessableEntity("Unable to update the Suite Agreement");
            }
        }

        /// <summary>
        /// Deletes the suite agreement with the specified identifier.
        /// </summary>
        /// <param name="suiteAgreementId">The unique identifier of the suite agreement to delete.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation. Returns 200 OK if the suite agreement
        /// was deleted successfully; 404 Not Found if no suite agreement with the specified identifier exists; or 422
        /// Unprocessable Entity if the deletion could not be completed due to a server error.</returns>
        [HttpDelete("{suiteAgreementId:long}")]
        [EndpointName("DeleteSuiteAgreementAsync")]
        public async Task<ActionResult> DeleteSuiteAgreementAsync([FromRoute] long suiteAgreementId)
        {
            try
            {
                var success = await _suiteAgreementService.DeleteSuiteAgreementAsync(suiteAgreementId);

                if (success)
                {
                    return Ok();
                }

                return NotFound($"Suite agreement with Id '{suiteAgreementId}' not found ");
            }
            catch (Exception)
            {
                // TODO: Return a server error
                return UnprocessableEntity("Unable to delete the Suite Agreement");
            }
        }

        /// <summary>
        /// Downloads the file associated with the specified suite agreement.
        /// </summary>
        /// <param name="suiteAgreementId">The unique identifier of the suite agreement whose file is to be downloaded.</param>
        /// <returns>An <see cref="FileContentResult"/> containing the file content if found; otherwise, a <see
        /// cref="NotFoundResult"/> if the suite agreement file does not exist.</returns>
        [HttpGet("{suiteAgreementId:long}/download")]
        [EndpointName("DownloadSuiteAgreementFileAsync")]
        [Produces("application/octet-stream")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileResult))]
        public async Task<ActionResult> DownloadSuiteAgreementFileAsync([FromRoute] long suiteAgreementId)
        {
            var agreementFile = await _suiteAgreementService.GetSuiteAgreementFileBySuiteAgreementIdAsync(suiteAgreementId);

            if (agreementFile == null)
            {
                return NotFound();
            }

            return File(agreementFile.Content, agreementFile.ContentType, agreementFile.FileName);
        }

        /// <summary>
        /// Downloads multiple suite agreement files as a single ZIP archive.
        /// </summary>
        /// <remarks>The ZIP archive will contain one entry for each suite agreement file found. The file
        /// name of the ZIP will include a timestamp to ensure uniqueness. If any of the specified IDs do not correspond
        /// to existing files, only the files that are found will be included in the archive.</remarks>
        /// <param name="suiteAgreementIds">A list of suite agreement IDs for which to retrieve and download files. Must not be null or empty.</param>
        /// <returns>An HTTP response containing a ZIP file with the requested suite agreement files if found; otherwise, a 400
        /// Bad Request if no IDs are provided, or a 404 Not Found if no files are found for the specified IDs.</returns>
        [HttpGet("download-batch")]
        [EndpointName("DownloadMultipleSuiteAgreementsAsync")]
        [Produces("application/octet-stream")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileResult))]
        public async Task<ActionResult> DownloadMultipleSuiteAgreementsAsync([FromQuery] List<long> suiteAgreementIds)
        {
            if (suiteAgreementIds == null || suiteAgreementIds.Count == 0)
            {
                return BadRequest("No IDs provided.");
            }

            var agreementFiles = await _suiteAgreementService.GetSuiteAgreementFilesBySuiteAgreementIdsAsync(suiteAgreementIds);

            if (agreementFiles == null || agreementFiles.Count == 0)
            {
                return NotFound("No files found for provided IDs.");
            }

            return new FileCallbackResult("application/zip", async (outputStream, _) =>
            {
                using var memoryStream = new MemoryStream();

                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true))
                {
                    foreach (var file in agreementFiles)
                    {
                        if (file != null)
                        {
                            var entry = archive.CreateEntry(file.FileName, CompressionLevel.Fastest);
                            using var entryStream = entry.Open();
                            await entryStream.WriteAsync(file.Content, 0, file.Content.Length);
                        }
                    }
                }

                memoryStream.Position = 0;
                await memoryStream.CopyToAsync(outputStream);
            })
            { FileDownloadName = $"Agreements_{DateTime.Now:yyyyMMddHHmm}.zip" };
        }
    }
}
