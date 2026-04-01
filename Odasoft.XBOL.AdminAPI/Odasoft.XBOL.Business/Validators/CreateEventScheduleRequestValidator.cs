using FluentValidation;
using Microsoft.Extensions.Localization;
using Odasoft.XBOL.DTO.Requests;

namespace Odasoft.XBOL.Business.Validators
{
    public class CreateEventScheduleRequestValidator : AbstractValidator<CreateEventScheduleRequest>
    {
        public CreateEventScheduleRequestValidator(IStringLocalizerFactory localizerFactory)
        {
            Include(new EventScheduleRequestValidator(localizerFactory));
        }
    }
}
