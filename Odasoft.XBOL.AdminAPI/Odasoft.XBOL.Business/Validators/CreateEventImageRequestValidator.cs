using FluentValidation;
using Microsoft.Extensions.Localization;
using Odasoft.XBOL.DTO.Requests;

namespace Odasoft.XBOL.Business.Validators
{
    public class CreateEventImageRequestValidator : AbstractValidator<CreateEventImageRequest>
    {
        public CreateEventImageRequestValidator(IStringLocalizerFactory localizerFactory)
        {
            Include(new EventImageRequestValidator(localizerFactory));
        }
    }
}
