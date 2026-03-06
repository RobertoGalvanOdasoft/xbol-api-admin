using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Data.Repositories;
using Odasoft.XBOL.DTO.Responses;

namespace Odasoft.XBOL.Business.Services
{
    public class PhoneRegionCodesService
    {
        private readonly PhoneRegionCodeRepository _phoneRegionCodeRepository;

        public PhoneRegionCodesService(PhoneRegionCodeRepository phoneRegionCodeRepository)
        {
            _phoneRegionCodeRepository = phoneRegionCodeRepository;
        }

        public async Task<List<PhoneRegionCodeResponse>> GetPhoneRegionCodesAsync()
        {
            var result = await _phoneRegionCodeRepository
                                    .Get()
                                    .AsNoTracking()
                                    .Select(c => new PhoneRegionCodeResponse
                                    {
                                        Id = c.Id,
                                        RegionCode = c.RegionCode,
                                        DialCode = c.DialCode,
                                        FlagEmoji = c.FlagEmoji
                                    }).ToListAsync();

            return result;
        }
    }
}
