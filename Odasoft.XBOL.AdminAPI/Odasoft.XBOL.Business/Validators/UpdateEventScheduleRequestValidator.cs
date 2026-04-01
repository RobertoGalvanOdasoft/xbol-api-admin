using FluentValidation;
using Microsoft.Extensions.Localization;
using Odasoft.XBOL.DTO.Requests;
using System.Reflection;

namespace Odasoft.XBOL.Business.Validators
{
    public class UpdateEventScheduleRequestValidator : AbstractValidator<UpdateEventScheduleRequest>
    {
        public UpdateEventScheduleRequestValidator(IStringLocalizerFactory localizerFactory)
        {
            var localizer = localizerFactory.Create(
                "SharedResource", Assembly.GetEntryAssembly()!.GetName().Name!);

            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage(string.Format(localizer["The {0} field must be greater than {1}."], localizer["Id"], 0));

            Include(new EventScheduleRequestValidator(localizerFactory));
        }
    }
}
