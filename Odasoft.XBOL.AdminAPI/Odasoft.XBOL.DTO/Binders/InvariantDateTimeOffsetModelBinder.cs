using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace Odasoft.XBOL.DTO.Binders
{
    public class InvariantDateTimeOffsetModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueResult == ValueProviderResult.None || string.IsNullOrWhiteSpace(valueResult.FirstValue))
            {
                return Task.CompletedTask;
            }

            // Parse using InvariantCulture to match NSwag's multipart form output
            if (DateTimeOffset.TryParse(valueResult.FirstValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            {
                bindingContext.Result = ModelBindingResult.Success(parsedDate);
            }
            else
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, $"The value '{valueResult.FirstValue}' is not valid for {bindingContext.ModelName}.");
            }

            return Task.CompletedTask;
        }
    }
}
