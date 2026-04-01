using FluentValidation;
using Microsoft.Extensions.Localization;
using Odasoft.XBOL.DTO.Requests;

namespace Odasoft.XBOL.Business.Validators
{
    public class EventImageRequestValidator : AbstractValidator<IEventImageRequest>
    {
        public EventImageRequestValidator(IStringLocalizerFactory localizerFactory)
        {
            var localizer = localizerFactory.Create("SharedResource", typeof(EventRequestValidator).Assembly.GetName().Name!);

            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage(string.Format(localizer["The {0} field is required."], localizer["Event"]));

            RuleFor(x => x.ImageType)
                .NotEmpty()
                .WithErrorCode(string.Format(localizer["The {0} field is required."], localizer["ImageType"]));

            RuleFor(x => x.Order)
                .NotEmpty()
                .WithMessage(string.Format(localizer["The {0} field is required."], localizer["Order"]));

            RuleFor(x => x.Image)
                .NotEmpty()
                .WithMessage(string.Format(localizer["The {0} field is required."], localizer["Image"]));
        }
    }
}
