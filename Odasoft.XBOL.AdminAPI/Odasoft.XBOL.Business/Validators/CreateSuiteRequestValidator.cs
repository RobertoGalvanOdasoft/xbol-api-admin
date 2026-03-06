using FluentValidation;
using Microsoft.Extensions.Localization;
using Odasoft.XBOL.DTO.Requests;

namespace Odasoft.XBOL.Business.Validators;

public class CreateSuiteRequestValidator : AbstractValidator<CreateSuiteRequest>
{
    public CreateSuiteRequestValidator(IStringLocalizerFactory localizerFactory)
    {
        Include(new SuiteRequestValidator(localizerFactory));
    }
}
