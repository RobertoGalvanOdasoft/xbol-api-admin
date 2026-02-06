using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Commons.Extensions;
using Odasoft.XBOL.Data.Repositories;
using Odasoft.XBOL.DTO.Requests;
using Odasoft.XBOL.DTO.Results;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Business.Services
{
    public class SuiteAgreementService
    {
        private readonly SuiteAgreementRepository _suiteAgreementRepository;
        private readonly SuiteAgreementFileRepository _suiteAgreementFileRepository;

        public SuiteAgreementService(SuiteAgreementRepository suiteAgreementRepository, SuiteAgreementFileRepository suiteAgreementFileRepository)
        {
            _suiteAgreementRepository = suiteAgreementRepository;
            _suiteAgreementFileRepository = suiteAgreementFileRepository;
        }

        public async Task<List<SuiteAgreementResult>> GetSuiteAgreementsAsync()
        {
            return await _suiteAgreementRepository
                            .Get()
                            .AsNoTracking()
                            .Select(sa => new SuiteAgreementResult
                            {
                                Id = sa.Id,
                                SuiteId = sa.Suite.Id,
                                SuiteName = sa.Suite.Name,
                                SuiteLevel = sa.Suite.SuiteLevel.Name,
                                OwnerName = sa.OwnerName,
                                OwnerEmail = sa.OwnerEmail,
                                OwnerPhone = sa.OwnerPhone,
                                StartDate = sa.StartDate,
                                EndDate = sa.EndDate,
                                FileName = sa.SuiteAgreementFile != null ? sa.SuiteAgreementFile.FileName : string.Empty
                            })
                            .ToListAsync();
        }

        public async Task<SuiteAgreementResult?> GetSuiteAgreementByIdAsync(long suiteAgreementId)
        {
            SuiteAgreement? suiteAgreement = await _suiteAgreementRepository.GetByIdAsync(suiteAgreementId);

            if (suiteAgreement == null)
            {
                return null;
            }

            return new SuiteAgreementResult
            {
                Id = suiteAgreement.Id,
                SuiteId = suiteAgreement.Suite.Id,
                SuiteName = suiteAgreement.Suite.Name,
                SuiteLevel = suiteAgreement.Suite.SuiteLevel.Name,
                OwnerName = suiteAgreement.OwnerName,
                OwnerEmail = suiteAgreement.OwnerEmail,
                OwnerPhone = suiteAgreement.OwnerPhone,
                StartDate = suiteAgreement.StartDate,
                EndDate = suiteAgreement.EndDate,
                FileName = suiteAgreement.SuiteAgreementFile != null ? suiteAgreement.SuiteAgreementFile.FileName : string.Empty
            };
        }

        public async Task<bool> CreateSuiteAgreementAsync(CreateSuiteAgreementRequest request)
        {
            try
            {
                var suiteAgreement = new SuiteAgreement
                {
                    SuiteId = request.SuiteId,
                    OwnerName = request.OwnerName,
                    OwnerEmail = request.OwnerEmail,
                    OwnerPhone = request.OwnerPhone,
                    StartDate = request.StartDate.ToUniversalTime(),
                    EndDate = request.EndDate.ToUniversalTime(),
                    CreatedBy = Guid.Empty,
                    UpdatedBy = Guid.Empty,
                    CreatedAt = DateTimeOffset.Now.ToUniversalTime(),
                    UpdatedAt = DateTimeOffset.Now.ToUniversalTime()
                };

                await _suiteAgreementRepository.InsertAsync(suiteAgreement);
                await _suiteAgreementRepository.CommitAsync();

                var fileContent = FileExtensions.ConvertIFormFileToByteArray(request.AgreementFile);

                if (fileContent != null)
                {
                    var suiteAgreementFile = new SuiteAgreementFile
                    {
                        SuiteAgreementId = suiteAgreement.Id,
                        FileName = request.AgreementFile.FileName,
                        ContentType = request.AgreementFile.ContentType,
                        Content = fileContent,
                        CreatedBy = Guid.Empty,
                        UpdatedBy = Guid.Empty,
                        CreatedAt = DateTimeOffset.Now.ToUniversalTime(),
                        UpdatedAt = DateTimeOffset.Now.ToUniversalTime()
                    };

                    await _suiteAgreementFileRepository.InsertAsync(suiteAgreementFile);
                    await _suiteAgreementFileRepository.CommitAsync();
                }
                else
                {
                    Console.WriteLine("Failed to convert agreement file to byte array.");
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating suite agreement: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateSuiteAgreementAsync(UpdateSuiteAgreementRequest request)
        {
            try
            {
                var suiteAgreement = await _suiteAgreementRepository.GetByIdAsync(request.Id);

                if (suiteAgreement == null)
                {
                    return false;
                }

                suiteAgreement.SuiteId = request.SuiteId;
                suiteAgreement.OwnerName = request.OwnerName;
                suiteAgreement.OwnerEmail = request.OwnerEmail;
                suiteAgreement.OwnerPhone = request.OwnerPhone;
                suiteAgreement.StartDate = request.StartDate.ToUniversalTime();
                suiteAgreement.EndDate = request.EndDate.ToUniversalTime();
                suiteAgreement.UpdatedAt = DateTimeOffset.Now.ToUniversalTime();
                suiteAgreement.UpdatedBy = Guid.Empty;

                await _suiteAgreementRepository.UpdateAsync(suiteAgreement);
                await _suiteAgreementRepository.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating suite agreement: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteSuiteAgreementAsync(long suiteAgreementId)
        {
            try
            {
                var suiteAgreementFile = await _suiteAgreementFileRepository
                                                .Get()
                                                .Where(x => x.SuiteAgreementId == suiteAgreementId)
                                                .FirstOrDefaultAsync();

                if (suiteAgreementFile is not null)
                {
                    await _suiteAgreementFileRepository.HardDeleteAsync(suiteAgreementFile);
                    await _suiteAgreementFileRepository.CommitAsync();
                }

                var suiteAgreement = await _suiteAgreementRepository.GetByIdAsync(suiteAgreementId);

                if (suiteAgreement is null)
                {
                    return false;
                }

                await _suiteAgreementRepository.HardDeleteAsync(suiteAgreement);
                await _suiteAgreementRepository.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting suite agreement: {ex.Message}");
                throw;
            }
        }

        public async Task<SuiteAgreementFile?> GetSuiteAgreementFileBySuiteAgreementIdAsync(long suiteAgreementId)
        {
            return await _suiteAgreementFileRepository
                            .Get()
                            .AsNoTracking()
                            .FirstOrDefaultAsync(saf => saf.SuiteAgreementId == suiteAgreementId);
        }

        public async Task<List<SuiteAgreementFile>> GetSuiteAgreementFilesBySuiteAgreementIdsAsync(List<long> suiteAgreementIds)
        {
            return await _suiteAgreementFileRepository
                                    .Get()
                                    .AsNoTracking()
                                    .Where(a => suiteAgreementIds.Contains(a.SuiteAgreementId))
                                    .ToListAsync();
        }
    }
}
