using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Odasoft.XBOL.Business;

namespace Odasoft.XBOL.AdminAPI.Filters;

public class ApiExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not ApiException apiException)
        {
            return;
        }

        context.Result = context.Exception is ApiException<SeatsIoErrorResponse> typed
            ? new ObjectResult(typed.Result) { StatusCode = typed.StatusCode }
            : new ObjectResult(apiException.Response) { StatusCode = apiException.StatusCode };

        context.ExceptionHandled = true;
    }
}
