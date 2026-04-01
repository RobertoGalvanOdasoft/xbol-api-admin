using FluentValidation;
using Microsoft.Extensions.Localization;
using Odasoft.XBOL.DTO.Requests;

namespace Odasoft.XBOL.Business.Validators
{
    public class CreateEventRequestValidator : AbstractValidator<CreateEventRequest>
    {
        public CreateEventRequestValidator(IStringLocalizerFactory localizerFactory)
        {
            Include(new EventRequestValidator(localizerFactory));
        }
    }
}
