using Odasoft.XBOL.Commons.Enums;
using Odasoft.XBOL.Data.Repositories.Season;
using XBOL.Admin.Core.DTO;

namespace Odasoft.XBOL.Business.Services
{
    public class SeasonPassService(SeasonPassRepository repository)
    {
        public async Task UpdateAsync(SeasonPassStatusRequest request)
        {
            var result = await repository.GetByIdAsync(request.SeasonPassId);
            if (result != null)
            {
                result.Status = request.SeasonPassStatus;
                result.SuspendedReason = request.SeasonPassSuspendedReason;

                if (request.SeasonPassSuspendedReason == SeasonPassSuspendedReason.Other)
                {
                    result.SuspendedOtherReason = request.SuspendedOtherReason?.Trim().ToUpper();
                }
                else
                {
                    result.SuspendedOtherReason = null;
                }

                await repository.UpdateAsync(result);
            }
        }
    }
}
