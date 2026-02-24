using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Data.Repositories;
using Odasoft.XBOL.DTO.Requests;
using Odasoft.XBOL.DTO.Results;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Business.Services
{
    public class LegalRepresentativeService
    {
        private readonly LegalRepresentativeRepository _legalRepresentativeRepository;

        public LegalRepresentativeService(LegalRepresentativeRepository legalRepresentativeService)
        {
            _legalRepresentativeRepository = legalRepresentativeService;
        }

        public async Task<LegalRepresentativeResult?> GetLegalRepresentativeByClientIdAsync(long clientId)
        {
            LegalRepresentative? legalRepresentative = await _legalRepresentativeRepository.Get()
                                                               .AsNoTracking()
                                                               .Where(x => x.ClientId == clientId)
                                                               .SingleOrDefaultAsync();

            if (legalRepresentative == null)
            {
                Console.WriteLine($"Legal representative with the Client Id {clientId} was not found.");
                return null;
            }

            return new LegalRepresentativeResult
            {
                Id = legalRepresentative.Id,
                ClientId = legalRepresentative.ClientId,
                FullName = legalRepresentative.FullName,
                DOB = legalRepresentative.DOB,
                TaxId = legalRepresentative.TaxId,
                CURP = legalRepresentative.CURP
            };
        }

        public async Task<bool> CreateLegalRepresentativeAsync(LegalRepresentativeRequest request)
        {
            var legalRepresentative = new LegalRepresentative
            {
                ClientId = request.ClientId,
                FullName = request.FullName,
                DOB = request.DOB.ToUniversalTime(),
                TaxId = request.TaxId,
                CURP = request.CURP
            };

            try
            {
                await _legalRepresentativeRepository.InsertAsync(legalRepresentative);
                await _legalRepresentativeRepository.CommitAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating a Legal representative: {ex.Message}");
                return false;
            }

            return true;
        }

        public async Task<bool> UpdateLegalRepresentativeByClientIdAsync(LegalRepresentativeRequest request)
        {
            LegalRepresentative? representative = _legalRepresentativeRepository.Get()
                                                    .Where(x => x.ClientId == request.ClientId)
                                                    .SingleOrDefault();

            if (representative == null)
            {
                Console.WriteLine($"Legal representative with Client Id {request.ClientId} not found.");
                return false;
            }

            representative.FullName = request.FullName;
            representative.DOB = request.DOB;
            representative.TaxId = request.TaxId;
            representative.CURP = request.CURP;

            try
            {
                await _legalRepresentativeRepository.UpdateAsync(representative);
                await _legalRepresentativeRepository.CommitAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating legal representative: {ex.Message}");
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteLegalRepresentativeByClientIdAsync(long clientId)
        {
            LegalRepresentative? legalRepresentative = _legalRepresentativeRepository.Get()
                                                    .Where(x => x.ClientId == clientId)
                                                    .SingleOrDefault();

            if (legalRepresentative == null)
            {
                Console.WriteLine($"Legal representative with Client Id {clientId} not found.");
                return false;
            }

            try
            {
                await _legalRepresentativeRepository.HardDeleteAsync(legalRepresentative);
                await _legalRepresentativeRepository.CommitAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting legal representative wiht Client Id {clientId}. Error: {ex.Message}");
                return false;
            }

            return true;
        }
    }
}
