using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Commons.Enums;
using Odasoft.XBOL.Commons.Helpers;
using Odasoft.XBOL.Commons.Requests.Filters;
using Odasoft.XBOL.Data.Repositories;
using Odasoft.XBOL.Data.Repositories.Client;
using Odasoft.XBOL.DTO.QueryParams;
using Odasoft.XBOL.DTO.Requests;
using Odasoft.XBOL.DTO.Response;
using Odasoft.XBOL.DTO.Results;
using Odasoft.XBOL.Models;
using XBOL.Admin.Core.DTO;

namespace Odasoft.XBOL.Business.Services
{
    public class ClientService(ClientRepository repository, ClientCreditAccountRepository clientCreditRepository, LegalRepresentativeRepository legalRepRepository)
    {
        public async Task<ClientSeasonEvent?> GetClientSeasonEventInfoAsync(ClientFilter filter)
        {
            return await repository.GetClientSeasonEventInfoAsync(filter);
        }

        public async Task<ClientResult> CreateClientAsync(CreateClientRequest request)
        {
            // This should be throw in the controller
            ArgumentNullException.ThrowIfNull(request);

            Client newClient = new()
            {
                ClientType = request.PersonTypeId ?? ClientType.Business,
                FullName = request.CompanyName,
                Gender = request.Gender,
                DateOfBirth = request.DateOfBirth?.ToUniversalTime(),
                BusinessName = request.SocialReason ?? request.CompanyName,
                Email = request.Email,
                PhoneRegionCodeId = request.PhoneRegionCodeId,
                PhoneNumber = PhoneNumberHelper.NormalizePhoneNumber(request.PhoneNumber ?? ""),
                TaxId = request.RFC,
                Country = request.Country,
                State = request.State,
                City = request.City,
                StreetAddress = request.Street,
                ExtNum = request.ExtNumber,
                IntNum = request.IntNumber,
                PostalCode = request.PostalCode,
                Neighborhood = request.Neighborhood,
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = Guid.Empty,
                UpdatedAt = DateTimeOffset.UtcNow,
                UpdatedBy = Guid.Empty
            };

            if (request.Credit is not null && request.HasCredit)
            {
                // TODO: Make fields nullable on db
                newClient.ClientCreditAccount = new ClientCreditAccount
                {
                    CreditLimit = request.Credit.AuthorizedAmount ?? 0,
                    StartDate = (request.Credit.StartDate ?? DateTimeOffset.UnixEpoch).ToUniversalTime(),
                    PaymentFrequency = request.Credit.PaymentCycleTypeId ?? PaymentFrequency.Monthly,
                    AppliesInterestRate = request.Credit.InterestApply ?? false,
                    CreditStatus = CreditStatus.Pending,
                    CurrentBalance = 0,
                    // TODO: Update data from logged user identity
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = Guid.Empty,
                    UpdatedAt = DateTimeOffset.UtcNow,
                    UpdatedBy = Guid.Empty,
                    IsActive = true,
                    EndDate = null,
                };
            }

            if (request.LegalRep is not null && (request.LegalRep.Name is not null || request.LegalRep.Birthday is not null || request.LegalRep.RFC is not null || request.LegalRep.CURP is not null))
            {
                newClient.LegalRepresentative = new LegalRepresentative
                {
                    FullName = request.LegalRep.Name ?? "",
                    DOB = (request.LegalRep.Birthday ?? DateTimeOffset.UnixEpoch).ToUniversalTime(),
                    TaxId = request.LegalRep.RFC ?? "",
                    CURP = request.LegalRep.CURP ?? ""
                };
            }

            await repository.InsertAsync(newClient);
            await repository.CommitAsync();

            // TODO: Change return type for credit detail?
            return new ClientResult
            {
                Id = newClient.Id,
                ClientName = newClient.BusinessName ?? newClient.FullName ?? "",
                LegalRepName = newClient.LegalRepresentative?.FullName,
                HasCredit = newClient.ClientCreditAccount is not null,
                CreditStatus = newClient.ClientCreditAccount?.CreditStatus,
                CreditAmount = newClient.ClientCreditAccount?.CreditLimit
            };
        }

        public async Task<bool> UpdateClientAsync(long id, UpdateClientRequest request)
        {
            Client? existingClient = await repository.GetByIdAsync(id);

            if (existingClient is null)
            {
                Console.WriteLine($"Client with ID {id} not found.");
                return false;
            }

            try
            {
                existingClient.ClientType = request.PersonTypeId ?? ClientType.Business;
                existingClient.FullName = request.CompanyName;
                existingClient.Gender = request.Gender;
                existingClient.DateOfBirth = request.DateOfBirth?.ToUniversalTime();
                existingClient.BusinessName = request.SocialReason;
                existingClient.Email = request.Email;
                existingClient.PhoneRegionCodeId = request.PhoneRegionCodeId;
                existingClient.PhoneNumber = PhoneNumberHelper.NormalizePhoneNumber(request.PhoneNumber ?? "");
                existingClient.TaxId = request.RFC;
                existingClient.Country = request.Country;
                existingClient.State = request.State;
                existingClient.City = request.City;
                existingClient.StreetAddress = request.Street;
                existingClient.ExtNum = request.ExtNumber;
                existingClient.IntNum = request.IntNumber;
                existingClient.PostalCode = request.PostalCode;
                existingClient.Neighborhood = request.Neighborhood;
                existingClient.IsActive = true;
                existingClient.UpdatedAt = DateTimeOffset.UtcNow;
                existingClient.UpdatedBy = Guid.Empty;

                await repository.UpdateAsync(existingClient);
                await repository.CommitAsync();

                if (request.Credit != null)
                {
                    ClientCreditAccount? clientCreditAccount = await clientCreditRepository.GetByIdAsync(request.Credit.Id ?? 0);

                    if (clientCreditAccount is null)
                    {
                        if (request.HasCredit)
                        {
                            clientCreditAccount = new()
                            {
                                ClientId = existingClient.Id,
                                CreditStatus = CreditStatus.Pending,
                                CurrentBalance = 0,
                                // TODO: Update data from logged user identity
                                UpdatedAt = DateTimeOffset.UtcNow,
                                UpdatedBy = Guid.Empty,
                                IsActive = true,
                                EndDate = null,
                                CreditLimit = request.Credit.AuthorizedAmount ?? 0,
                                StartDate = (request.Credit.StartDate ?? DateTimeOffset.UnixEpoch).ToUniversalTime(),
                                PaymentFrequency = request.Credit.PaymentCycleTypeId ?? PaymentFrequency.Monthly,
                                AppliesInterestRate = request.Credit.InterestApply ?? false
                            };

                            await clientCreditRepository.InsertAsync(clientCreditAccount);
                            await clientCreditRepository.CommitAsync();
                        }
                    }
                    else
                    {
                        if (request.HasCredit)
                        {
                            clientCreditAccount.CreditLimit = request.Credit.AuthorizedAmount ?? 0;
                            clientCreditAccount.StartDate = (request.Credit.StartDate ?? DateTimeOffset.UnixEpoch).ToUniversalTime();
                            clientCreditAccount.PaymentFrequency = request.Credit.PaymentCycleTypeId ?? PaymentFrequency.Monthly;
                            clientCreditAccount.AppliesInterestRate = request.Credit.InterestApply ?? false;
                        }
                        else
                        {
                            clientCreditAccount.IsActive = false;
                        }

                        await clientCreditRepository.UpdateAsync(clientCreditAccount);
                        await clientCreditRepository.CommitAsync();
                    }
                }

                if (request.LegalRep != null)
                {
                    LegalRepresentative? legalRep = await legalRepRepository.GetByIdAsync(request.LegalRep.Id ?? 0);

                    if (legalRep is null)
                    {
                        legalRep = new()
                        {
                            ClientId = existingClient.Id,
                            FullName = request.LegalRep.Name ?? "",
                            DOB = (request.LegalRep.Birthday ?? DateTimeOffset.UnixEpoch).ToUniversalTime(),
                            TaxId = request.LegalRep.RFC ?? "",
                            CURP = request.LegalRep.CURP ?? ""
                        };

                        await legalRepRepository.InsertAsync(legalRep);
                        await legalRepRepository.CommitAsync();
                    }
                    else
                    {
                        legalRep.FullName = request.LegalRep.Name ?? "";
                        legalRep.DOB = (request.LegalRep.Birthday ?? DateTimeOffset.UnixEpoch).ToUniversalTime();
                        legalRep.TaxId = request.LegalRep.RFC ?? "";
                        legalRep.CURP = request.LegalRep.CURP ?? "";

                        await legalRepRepository.UpdateAsync(legalRep);
                        await legalRepRepository.CommitAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO: Implement proper logging
                Console.WriteLine($"Error updating Client: {ex.Message}");
                return false;
            }

            return true;
        }

        public async Task DeleteClientAsync(long id)
        {
            Client? client = await repository.GetByIdAsync(id);

            if (client is null || !client.IsActive)
            {
                throw new KeyNotFoundException();
            }

            // Create soft delete method on base repository
            client.IsActive = false;

            await repository.CommitAsync();
        }

        public async Task<PagedResponse<ClientResult>> GetClientsAsync(ClientsQueryParams queryParams)
        {
            IQueryable<ClientResult> query = repository.Get()
                            .AsNoTracking()
                            .Where(c => c.IsActive && c.ClientType == ClientType.Business)
                            .Select(c => new ClientResult
                            {
                                Id = c.Id,
                                ClientName = c.BusinessName ?? c.FullName ?? "",
                                LegalRepName = c.LegalRepresentative != null ? c.LegalRepresentative.FullName : null,
                                HasCredit = c.ClientCreditAccount != null,
                                CreditAmount = c.ClientCreditAccount != null ? c.ClientCreditAccount.CreditLimit : null,
                                PendingAmount = (c.ClientCreditAccount == null ? 0
                                                : c.ClientCreditAccount.ClientCreditTransactions
                                                .Where(ct => ct.TransactionType == CreditTransactionType.Drawdown
                                                    || ct.TransactionType == CreditTransactionType.Fee
                                                    || ct.TransactionType == CreditTransactionType.AdjustmentCredit)
                                                .Sum(x => x.Amount))
                                                - (c.ClientCreditAccount == null ? 0
                                                : c.ClientCreditAccount.ClientCreditTransactions
                                                .Where(ct => ct.TransactionType == CreditTransactionType.Payment
                                                    || ct.TransactionType == CreditTransactionType.Reversal
                                                    || ct.TransactionType == CreditTransactionType.AdjustmentCredit)
                                                .Sum(x => x.Amount)),
                                AmountPaid = c.ClientCreditAccount != null ? c.ClientCreditAccount.ClientCreditTransactions.Where(t => t.TransactionType == Commons.Enums.CreditTransactionType.Payment).Sum(t => t.Amount) : null,
                                CreditStatus = c.ClientCreditAccount != null ? c.ClientCreditAccount.CreditStatus : null
                            });

            SetCreditStatusFilter(ref query, queryParams.CreditStatus);
            SetHasCreditFilter(ref query, queryParams.HasCredit);
            SetSearchTermFilter(ref query, queryParams.SearchTerm);
            SetOrder(ref query, queryParams.SortBy, queryParams.Descending);

            List<ClientResult> clients = await query.ToListAsync();

            int totalCount = clients.Count;

            return new PagedResponse<ClientResult>
            {
                Items = clients
                        .Skip((queryParams.Page) * queryParams.PageSize)
                        .Take(queryParams.PageSize).ToList(),
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };
        }

        public Task<List<ClientResult>> GetClientsListByIdAsync(IEnumerable<long> ids)
        {
            IQueryable<ClientResult> query = repository.Get()
                            .AsNoTracking()
                            .Where(c => ids.Contains(c.Id))
                            .Select(c => new ClientResult
                            {
                                Id = c.Id,
                                ClientName = string.IsNullOrWhiteSpace(c.BusinessName) ? (c.FullName ?? "") : c.BusinessName,
                                LegalRepName = c.LegalRepresentative != null ? c.LegalRepresentative.FullName : null,
                                HasCredit = c.ClientCreditAccount != null,
                                CreditAmount = c.ClientCreditAccount != null ? c.ClientCreditAccount.CreditLimit : null,
                                PendingAmount = (c.ClientCreditAccount == null ? 0
                                                : c.ClientCreditAccount.ClientCreditTransactions
                                                .Where(ct => ct.TransactionType == CreditTransactionType.Drawdown
                                                    || ct.TransactionType == CreditTransactionType.Fee
                                                    || ct.TransactionType == CreditTransactionType.AdjustmentCredit)
                                                .Sum(x => x.Amount))
                                                - (c.ClientCreditAccount == null ? 0
                                                : c.ClientCreditAccount.ClientCreditTransactions
                                                .Where(ct => ct.TransactionType == CreditTransactionType.Payment
                                                    || ct.TransactionType == CreditTransactionType.Reversal
                                                    || ct.TransactionType == CreditTransactionType.AdjustmentCredit)
                                                .Sum(x => x.Amount)),
                                AmountPaid = c.ClientCreditAccount != null
                                                ? c.ClientCreditAccount.ClientCreditTransactions
                                                    .Where(t => t.TransactionType == Commons.Enums.CreditTransactionType.Payment).Sum(t => t.Amount)
                                                : null,
                                CreditStatus = c.ClientCreditAccount != null ? c.ClientCreditAccount.CreditStatus : null
                            });

            return query.ToListAsync();
        }

        public async Task<ClientDetailResult?> GetClientDetailByIdAsync(long clientId)
        {
            ClientDetailResult? result = await repository.Get()
                .AsNoTracking()
                .Where(c => c.Id == clientId)
                .Select(c => new ClientDetailResult
                {
                    Id = c.Id,
                    ClientType = c.ClientType,
                    ClientName = c.FullName,
                    Gender = c.Gender,
                    DateOfBirth = c.DateOfBirth,
                    BusinessName = c.BusinessName,
                    Email = c.Email,
                    PhoneRegionCodeId = c.PhoneRegionCodeId,
                    DialCode = c.PhoneRegionCode == null
                                ? ""
                                : c.PhoneRegionCode.DialCode ?? "",
                    PhoneNumber = c.PhoneNumber,
                    TaxId = c.TaxId,
                    Country = c.Country,
                    State = c.State,
                    City = c.City,
                    StreetAddress = c.StreetAddress,
                    ExtNum = c.ExtNum,
                    IntNum = c.IntNum,
                    PostalCode = c.PostalCode,
                    Neighborhood = c.Neighborhood,
                    ClientCredit = c.ClientCreditAccount != null ? new ClientCreditDTO
                    {
                        Id = c.ClientCreditAccount.Id,
                        ClientId = c.Id,
                        CreditLimit = c.ClientCreditAccount.CreditLimit,
                        AppliesInterestRate = c.ClientCreditAccount.AppliesInterestRate,
                        PaymentFrequency = c.ClientCreditAccount.PaymentFrequency,
                        PendingAmount = (c.ClientCreditAccount == null ? 0
                                                : c.ClientCreditAccount.ClientCreditTransactions
                                                .Where(ct => ct.TransactionType == CreditTransactionType.Drawdown
                                                    || ct.TransactionType == CreditTransactionType.Fee
                                                    || ct.TransactionType == CreditTransactionType.AdjustmentCredit)
                                                .Sum(x => x.Amount))
                                                - (c.ClientCreditAccount == null ? 0
                                                : c.ClientCreditAccount.ClientCreditTransactions
                                                .Where(ct => ct.TransactionType == CreditTransactionType.Payment
                                                    || ct.TransactionType == CreditTransactionType.Reversal
                                                    || ct.TransactionType == CreditTransactionType.AdjustmentCredit)
                                                .Sum(x => x.Amount)),
                        CreditStatus = c.ClientCreditAccount == null
                                        ? CreditStatus.Pending
                                        : c.ClientCreditAccount.CreditStatus,
                        AmountPaid = c.ClientCreditAccount == null
                                        ? 0m
                                        : c.ClientCreditAccount.ClientCreditTransactions.Where(t => t.TransactionType == CreditTransactionType.Payment).Sum(t => t.Amount),
                        StartDate = c.ClientCreditAccount == null
                                    ? DateTimeOffset.UtcNow
                                    : c.ClientCreditAccount.StartDate,
                        EndDate = c.ClientCreditAccount == null
                                    ? null
                                    : c.ClientCreditAccount.EndDate
                    } : null,
                    LegalRepresentative = c.LegalRepresentative != null ? new LegalRepresentativeDTO
                    {
                        Id = c.LegalRepresentative.Id,
                        ClientId = c.Id,
                        FullName = c.LegalRepresentative.FullName,
                        DOB = c.LegalRepresentative.DOB,
                        TaxId = c.LegalRepresentative.TaxId,
                        CURP = c.LegalRepresentative.CURP
                    } : null
                }).SingleOrDefaultAsync();

            //TODO: Use localization to get enum label
            result?.ClientCredit?.PaymentFrequencyLabel = result.ClientCredit.PaymentFrequency.ToString();
            result?.ClientCredit?.CreditStatusLabel = result.ClientCredit.CreditStatus.ToString();

            return result;
        }

        // TODO:
        // 1- Consider if this method should be in a separate service, as it might be used in other contexts (e.g., during client creation or update)
        // 2- Also consider that phone and email are not unique identifiers, so this method might return multiple results in some cases.
        // We might want to return a list of matches instead of a single result, or we might want to enforce that only one of the parameters is provided at a time.
        // 3- If we are using an email as an identifier, we should consider normalizing it (e.g., converting to lowercase) before searching, to avoid case sensitivity issues.
        // 4- If we are using a phone number as an identifier, we should consider normalizing it as well (e.g., removing spaces, dashes, or country codes) before searching, to improve matching accuracy.
        // 5- We should also consider the performance implications of this method, especially if the clients table is large.
        // We might want to add indexes on the phone and email columns to speed up the search.
        public async Task<ClientContactResponse?> SearchClientAsync(long? phoneRegionCodeId, string phoneNumber, string email)
        {
            ClientContactResponse? result = await repository.Get()
                .AsNoTracking()
                .Where(c => c.IsActive
                    && ((phoneRegionCodeId.HasValue
                            ? ((c.PhoneNumber != null && c.PhoneNumber == phoneNumber) && c.PhoneRegionCodeId == phoneRegionCodeId.Value)
                            : (c.PhoneNumber != null && c.PhoneNumber == phoneNumber)
                            )
                        || (email != null && c.Email == email))
                        )
                .Select(c => new ClientContactResponse
                {
                    Id = c.Id,
                    Name = c.FullName ?? "",
                    Email = c.Email ?? "",
                    PhoneRegionCodeId = c.PhoneRegionCodeId,
                    PhoneNumber = c.PhoneNumber ?? ""
                }).SingleOrDefaultAsync();

            return result;
        }

        private void SetHasCreditFilter(ref IQueryable<ClientResult> query, bool? hasCredit)
        {
            query = hasCredit switch
            {
                true => query.Where(x => x.HasCredit),
                false => query.Where(x => !x.HasCredit),
                _ => query
            };
        }

        private void SetCreditStatusFilter(ref IQueryable<ClientResult> query, List<CreditStatus>? creditStatus)
        {
            if (creditStatus is not null && creditStatus.Count != 0)
            {
                query = query.Where(c => c.CreditStatus != null && creditStatus.Contains(c.CreditStatus.Value));
            }
        }

        private void SetSearchTermFilter(ref IQueryable<ClientResult> query, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm) == false)
            {
                string lowerSearchTerm = searchTerm.ToLower();
                query = query.Where(x => x.ClientName.ToLower().Contains(lowerSearchTerm)
                            || (x.LegalRepName != null && x.LegalRepName.ToLower().Contains(lowerSearchTerm)));
            }
        }

        private void SetOrder(ref IQueryable<ClientResult> query, string sortBy, bool descending)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return;
            }

            query = sortBy.ToLower() switch
            {
                "name" => descending ? query.OrderByDescending(x => x.ClientName) : query.OrderBy(x => x.ClientName),
                "legalrepresentative" => descending ? query.OrderByDescending(x => x.LegalRepName) : query.OrderBy(x => x.LegalRepName),
                "credit" => descending ? query.OrderByDescending(x => x.CreditAmount) : query.OrderBy(x => x.CreditAmount),
                "pendingcredit" => descending ? query.OrderByDescending(x => x.PendingAmount) : query.OrderBy(x => x.PendingAmount),
                "amountpaid" => descending ? query.OrderByDescending(x => x.AmountPaid) : query.OrderBy(x => x.AmountPaid),
                "creditstatus" => descending ? query.OrderByDescending(x => x.CreditStatus) : query.OrderBy(x => x.CreditStatus),
                _ => query
            };
        }
    }

    // TODO: Move to appropiate place
    public sealed class ClientCsvMap : ClassMap<ClientResult>
    {
        // TODO: Use localization for column names and status enum
        public ClientCsvMap()
        {
            Map(m => m.ClientName).Name("Empresa");
            Map(m => m.LegalRepName).Name("Representante legal");
            Map(m => m.CreditAmount).Name("Crédito");
            Map(m => m.PendingAmount).Name("Pendiente");
            Map(m => m.AmountPaid).Name("Monto pagado");
            Map(m => m.CreditStatus).Name("Estatus");
        }
    }
}
