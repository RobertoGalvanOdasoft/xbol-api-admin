using FluentValidation;
using Microsoft.Extensions.Localization;
using Odasoft.XBOL.DTO.Requests;

namespace Odasoft.XBOL.Business.Validators
{
    public class EventRequestValidator : AbstractValidator<IEventRequest>
    {
        // TODO: Add MaxLength validations
        public EventRequestValidator(IStringLocalizerFactory localizerFactory)
        {
            var localizer = localizerFactory.Create("SharedResource", typeof(EventRequestValidator).Assembly.GetName().Name!);

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(string.Format(localizer["The {0} field is required."], localizer["Name"]))
                .MaximumLength(200)
                .WithMessage(string.Format(localizer["The {0} field must not exceed {1} characters."], localizer["Name"], 200));

            RuleFor(x => x.Category)
                .NotEmpty()
                .WithMessage(string.Format(localizer["The {0} field is required."], localizer["Category"]));

            RuleFor(x => x.VenueMapId)
                .NotEmpty()
                .WithMessage(string.Format(localizer["The {0} field is required."], localizer["Venue"]))
                .GreaterThan(0)
                .WithMessage(string.Format(localizer["The {0} field is required."], localizer["Venue"]));
        }
    }
}
