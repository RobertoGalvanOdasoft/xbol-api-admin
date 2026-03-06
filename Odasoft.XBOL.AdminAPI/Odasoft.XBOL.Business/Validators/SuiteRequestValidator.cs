using FluentValidation;
using Microsoft.Extensions.Localization;
using Odasoft.XBOL.DTO.Requests;
using System.Reflection;

namespace Odasoft.XBOL.Business.Validators;

public class SuiteRequestValidator : AbstractValidator<ISuiteRequest>
{
    public SuiteRequestValidator(IStringLocalizerFactory localizerFactory)
    {
        var localizer = localizerFactory.Create(
            "SharedResource", Assembly.GetEntryAssembly()!.GetName().Name!);

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(string.Format(localizer["The {0} field is required."], localizer["Name"]))
            .MaximumLength(200)
            .WithMessage(string.Format(localizer["The {0} field must not exceed {1} characters."], localizer["Name"], 200));

        RuleFor(x => x.SuiteLevelId)
            .GreaterThan(0)
            .WithMessage(string.Format(localizer["The {0} field must be greater than {1}."], localizer["SuiteLevelId"], 0));

        RuleFor(x => x.Seats)
            .GreaterThan(0)
            .WithMessage(string.Format(localizer["The {0} field must be greater than {1}."], localizer["Seats"], 0));
    }
}
