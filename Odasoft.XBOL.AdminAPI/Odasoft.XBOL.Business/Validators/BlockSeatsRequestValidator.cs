using FluentValidation;
using Microsoft.Extensions.Localization;
using Odasoft.XBOL.DTO.Requests;
using System.Reflection;

namespace Odasoft.XBOL.Business.Validators;

public class BlockSeatsRequestValidator : AbstractValidator<BlockSeatsRequest>
{
    public BlockSeatsRequestValidator(IStringLocalizerFactory localizerFactory)
    {
        var localizer = localizerFactory.Create(
            "SharedResource", Assembly.GetEntryAssembly()!.GetName().Name!);

        RuleFor(x => x.SeatKeys)
            .NotEmpty()
            .WithMessage(string.Format(localizer["The {0} field is required."], localizer["SeatKeys"]));

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage(string.Format(localizer["The {0} field is required."], localizer["Reason"]));

        RuleFor(x => x.Color)
            .NotEmpty()
            .WithMessage(string.Format(localizer["The {0} field is required."], localizer["Color"]));
    }
}
