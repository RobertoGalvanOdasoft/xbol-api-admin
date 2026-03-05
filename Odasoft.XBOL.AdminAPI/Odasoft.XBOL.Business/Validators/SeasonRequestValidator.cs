using FluentValidation;
using Microsoft.Extensions.Localization;
using Odasoft.XBOL.DTO.Requests;
using System.Reflection;

namespace Odasoft.XBOL.Business.Validators;

public class SeasonRequestValidator : AbstractValidator<ISeasonRequest>
{
    public SeasonRequestValidator(IStringLocalizerFactory localizerFactory)
    {
        var localizer = localizerFactory.Create(
            "SharedResource", Assembly.GetEntryAssembly()!.GetName().Name!);

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(string.Format(localizer["The {0} field is required."], localizer["Name"]));

        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage(string.Format(localizer["The {0} field is required."], localizer["Code"]));

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage(string.Format(localizer["The {0} field is required."], localizer["StartDate"]));

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage(string.Format(localizer["The {0} field is required."], localizer["EndDate"]));

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .When(x => x.EndDate.HasValue && x.StartDate.HasValue)
            .WithMessage(string.Format(localizer["DateGreaterThan"], localizer["EndDate"], localizer["StartDate"]));

        RuleFor(x => x.PreSaleDate)
            .GreaterThan(x => x.PublishedDate)
            .When(x => x.PreSaleDate.HasValue && x.PublishedDate.HasValue)
            .WithMessage(string.Format(localizer["DateGreaterThan"], localizer["PreSaleDate"], localizer["PublishedDate"]));

        RuleFor(x => x.OnSaleDate)
            .GreaterThan(x => x.PreSaleDate)
            .When(x => x.OnSaleDate.HasValue && x.PreSaleDate.HasValue)
            .WithMessage(string.Format(localizer["DateGreaterThan"], localizer["OnSaleDate"], localizer["PreSaleDate"]));

        RuleFor(x => x.OffSaleDate)
            .GreaterThan(x => x.OnSaleDate)
            .When(x => x.OffSaleDate.HasValue && x.OnSaleDate.HasValue)
            .WithMessage(string.Format(localizer["DateGreaterThan"], localizer["OffSaleDate"], localizer["OnSaleDate"]));
    }
}
