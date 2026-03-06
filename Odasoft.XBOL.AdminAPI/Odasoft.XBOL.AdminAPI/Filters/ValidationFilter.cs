using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;

namespace Odasoft.XBOL.AdminAPI.Filters;

public class ValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var (_, value) in context.ActionArguments)
        {
            if (value is null)
            {
                continue;
            }

            var validatorType = typeof(IValidator<>).MakeGenericType(value.GetType());

            if (context.HttpContext.RequestServices.GetService(validatorType) is not IValidator validator)
            {
                continue;
            }

            var validationResult = await validator.ValidateAsync(
                new ValidationContext<object>(value));

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                var localizer = context.HttpContext.RequestServices
                    .GetRequiredService<IStringLocalizerFactory>()
                    .Create(typeof(SharedResource));

                context.Result = new BadRequestObjectResult(
                    new ValidationProblemDetails(errors)
                    {
                        Title = localizer["ValidationTitle"]
                    });
                return;
            }
        }

        await next();
    }
}
