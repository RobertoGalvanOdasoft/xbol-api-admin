using FluentValidation;
using Microsoft.Extensions.Localization;
using Odasoft.XBOL.DTO.Requests;

namespace Odasoft.XBOL.Business.Validators;

public class UpdateSeasonRequestValidator : AbstractValidator<UpdateSeasonRequest>
{
    public UpdateSeasonRequestValidator(IStringLocalizerFactory localizerFactory)
    {
        Include(new SeasonRequestValidator(localizerFactory));
    }
}
