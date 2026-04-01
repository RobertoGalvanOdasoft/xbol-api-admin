using FluentValidation;
using Microsoft.Extensions.Localization;
using Odasoft.XBOL.DTO.Requests;

namespace Odasoft.XBOL.Business.Validators
{
    public class EventScheduleRequestValidator : AbstractValidator<IEventScheduleRequest>
    {
        public EventScheduleRequestValidator(IStringLocalizerFactory localizerFactory)
        {
            var localizer = localizerFactory.Create(
                "SharedResource", System.Reflection.Assembly.GetEntryAssembly()!.GetName().Name!);

            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage(string.Format(localizer["The {0} field is required."], localizer["Event"]));

            RuleFor(x => x.StartDateTime)
                .NotEmpty()
                .WithMessage(string.Format(localizer["The {0} field is required."], localizer["StartDateTime"]));

            RuleFor(x => x.EndDateTime)
                .NotEmpty()
                .WithMessage(string.Format(localizer["The {0} field is required."], localizer["EndDateTime"]));

            RuleFor(x => x.PreSaleDate)
                .NotEmpty()
                .WithMessage(string.Format(localizer["The {0} field is required."], localizer["PreSaleDate"]));

            RuleFor(x => x.PreSaleEndDate)
                .NotEmpty()
                .WithMessage(string.Format(localizer["The {0} field is required."], localizer["PreSaleEndDate"]));

            RuleFor(x => x.OnSaleDate)
                .NotEmpty()
                .WithMessage(string.Format(localizer["The {0} field is required."], localizer["OnSaleDate"]));

            RuleFor(x => x.OffSaleDate)
                .NotEmpty()
                .WithMessage(string.Format(localizer["The {0} field is required."], localizer["OffSaleDate"]));

            RuleFor(x => x.PublishedDate)
                .NotEmpty()
                .WithMessage(string.Format(localizer["The {0} field is required."], localizer["PublishedDate"]));

            RuleFor(x => x.GateOpenDate)
                .NotEmpty()
                .WithMessage(string.Format(localizer["The {0} field is required."], localizer["GateOpenDate"]));
        }
    }
}
