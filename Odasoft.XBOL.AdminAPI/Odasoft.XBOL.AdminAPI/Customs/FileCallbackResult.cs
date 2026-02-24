using Microsoft.AspNetCore.Mvc;

namespace Odasoft.XBOL.AdminAPI.Customs
{
    /// <summary>
    /// Represents an HTTP file response whose content is provided by a user-supplied callback that writes directly to
    /// the response stream.
    /// </summary>
    /// <remarks>Use this class to return file content that is generated or streamed on demand, rather than
    /// served from a static file. The callback is invoked during response execution and is responsible for writing the
    /// file data to the provided stream. This is useful for scenarios such as dynamically generated files or large
    /// files that should be streamed to the client without buffering the entire content in memory.</remarks>
    public class FileCallbackResult : FileResult
    {
        private readonly Func<Stream, ActionContext, Task> _callback;

        public FileCallbackResult(string contentType, Func<Stream, ActionContext, Task> callback)
            : base(contentType)
        {
            _callback = callback;
        }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            var response = context.HttpContext.Response;
            response.ContentType = ContentType;

            // This triggers the download dialog in the browser
            if (!string.IsNullOrWhiteSpace(FileDownloadName))
            {
                var headerValue = new Microsoft.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                headerValue.SetHttpFileName(FileDownloadName);
                response.Headers.ContentDisposition = headerValue.ToString();
            }

            await _callback(response.Body, context);
        }
    }
}
