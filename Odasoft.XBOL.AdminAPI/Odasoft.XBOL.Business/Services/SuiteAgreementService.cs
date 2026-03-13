using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Odasoft.XBOL.Commons.Extensions;
using Odasoft.XBOL.Commons.Helpers;
using Odasoft.XBOL.Commons.Options;
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
        private readonly FileUploadOptions _fileUploadOptions;

        public SuiteAgreementService(
            SuiteAgreementRepository suiteAgreementRepository,
            SuiteAgreementFileRepository suiteAgreementFileRepository,
            IOptions<FileUploadOptions> fileUploadOptions)
        {
            _suiteAgreementRepository = suiteAgreementRepository;
            _suiteAgreementFileRepository = suiteAgreementFileRepository;
            _fileUploadOptions = fileUploadOptions.Value;
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
                                SuiteLevelId = sa.Suite.SuiteLevel.Id,
                                SuiteLevel = sa.Suite.SuiteLevel.Name,
                                OwnerName = sa.OwnerName,
                                Email = sa.Email,
                                PhoneRegionCodeId = sa.PhoneRegionCodeId,
                                DialCode = sa.PhoneRegionCode != null ? sa.PhoneRegionCode.DialCode : "",
                                PhoneNumber = sa.PhoneNumber,
                                StartDate = sa.StartDate,
                                EndDate = sa.EndDate,
                                FileName = sa.SuiteAgreementFile != null ? sa.SuiteAgreementFile.FileName : ""
                            })
                            .ToListAsync();
        }

        public async Task<SuiteAgreementResult?> GetSuiteAgreementByIdAsync(long suiteAgreementId)
        {
            SuiteAgreement? suiteAgreement = await _suiteAgreementRepository
                                                    .Get()
                                                    .Include(x => x.Suite)
                                                        .ThenInclude(x => x.SuiteLevel)
                                                    .Include(x => x.SuiteAgreementFile)
                                                    .Include(x => x.PhoneRegionCode)
                                                    .AsNoTracking()
                                                    .Where(x => x.Id == suiteAgreementId)
                                                    .FirstOrDefaultAsync();

            if (suiteAgreement == null)
            {
                return null;
            }

            return new SuiteAgreementResult
            {
                Id = suiteAgreement.Id,
                SuiteId = suiteAgreement.Suite.Id,
                SuiteName = suiteAgreement.Suite.Name,
                SuiteLevelId = suiteAgreement.Suite.SuiteLevel.Id,
                SuiteLevel = suiteAgreement.Suite.SuiteLevel.Name,
                OwnerName = suiteAgreement.OwnerName,
                Email = suiteAgreement.Email,
                PhoneRegionCodeId = suiteAgreement.PhoneRegionCodeId,
                DialCode = suiteAgreement.PhoneRegionCode != null ? suiteAgreement.PhoneRegionCode.DialCode : "",
                PhoneNumber = suiteAgreement.PhoneNumber,
                StartDate = suiteAgreement.StartDate,
                EndDate = suiteAgreement.EndDate,
                FileName = suiteAgreement.SuiteAgreementFile != null ? suiteAgreement.SuiteAgreementFile.FileName : ""
            };
        }

        public async Task<long> CreateSuiteAgreementAsync(CreateSuiteAgreementRequest request)
        {
            try
            {
                var suiteAgreement = new SuiteAgreement
                {
                    SuiteId = request.SuiteId,
                    OwnerName = request.OwnerName,
                    Email = request.Email,
                    PhoneRegionCodeId = request.PhoneRegionCodeId,
                    PhoneNumber = PhoneNumberHelper.NormalizePhoneNumber(request.PhoneNumber),
                    StartDate = request.StartDate.ToUniversalTime(),
                    EndDate = request.EndDate.ToUniversalTime(),
                    CreatedBy = Guid.Empty,
                    UpdatedBy = Guid.Empty,
                    CreatedAt = DateTimeOffset.Now.ToUniversalTime(),
                    UpdatedAt = DateTimeOffset.Now.ToUniversalTime()
                };

                await _suiteAgreementRepository.InsertAsync(suiteAgreement);
                await _suiteAgreementRepository.CommitAsync();

                return suiteAgreement.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating suite agreement: {ex.Message}");
                return 0;
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
                suiteAgreement.Email = request.Email;
                suiteAgreement.PhoneRegionCodeId = request.PhoneRegionCodeId;
                suiteAgreement.PhoneNumber = PhoneNumberHelper.NormalizePhoneNumber(request.PhoneNumber);
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

        public async Task<bool> SaveSuiteAgreementFileBySuiteAgreementIdAsync(long suiteAgreementId, IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);
            if (!_fileUploadOptions.AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }

            try
            {
                var fileContent = FileExtensions.ConvertIFormFileToByteArray(file);

                if (fileContent != null)
                {
                    var suiteAgreementFile = new SuiteAgreementFile
                    {
                        SuiteAgreementId = suiteAgreementId,
                        FileName = file.FileName,
                        ContentType = file.ContentType,
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
                Console.WriteLine($"Error while trying to save agreement file: {ex.ToString()}");
                return false;
            }
        }
    }
}
