using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Commons.Enums;
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
        public async Task<ClientSeasonEvent> GetClientSeasonEventInfoAsync(ClientFilter filter)
        {
            return await repository.GetClientSeasonEventInfoAsync(filter);
        }

        public async Task<ClientResult> CreateClientAsync(CreateClientRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            Client newClient = new()
            {
                ClientType = request.PersonTypeId ?? ClientType.Business,
                FullName = request.CompanyName,
                BusinessName = request.SocialReason,
                Email = request.Email,
                CountryPhoneCode = request.CountryPhoneCode,
                PhoneNumber = request.Phone,
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
                    FullName = request.LegalRep.Name ?? string.Empty,
                    DOB = (request.LegalRep.Birthday ?? DateTimeOffset.UnixEpoch).ToUniversalTime(),
                    TaxId = request.LegalRep.RFC ?? string.Empty,
                    CURP = request.LegalRep.CURP ?? string.Empty
                };
            }

            await repository.InsertAsync(newClient);
            await repository.CommitAsync();

            // TODO: Change return type for credit detail?
            return new ClientResult
            {
                Id = newClient.Id,
                ClientName = newClient.BusinessName ?? newClient.FullName ?? string.Empty,
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
                existingClient.BusinessName = request.SocialReason;
                existingClient.Email = request.Email;
                existingClient.CountryPhoneCode = request.CountryPhoneCode;
                existingClient.PhoneNumber = request.Phone;
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
                }

                await clientCreditRepository.CommitAsync();

                LegalRepresentative? legalRep = await legalRepRepository.GetByIdAsync(request.LegalRep.Id ?? 0);


                if (legalRep is null)
                {
                    legalRep = new()
                    {
                        ClientId = existingClient.Id,
                        FullName = request.LegalRep.Name ?? string.Empty,
                        DOB = (request.LegalRep.Birthday ?? DateTimeOffset.UnixEpoch).ToUniversalTime(),
                        TaxId = request.LegalRep.RFC ?? string.Empty,
                        CURP = request.LegalRep.CURP ?? string.Empty
                    };

                    await legalRepRepository.InsertAsync(legalRep);
                }
                else
                {
                    legalRep.FullName = request.LegalRep.Name ?? string.Empty;
                    legalRep.DOB = (request.LegalRep.Birthday ?? DateTimeOffset.UnixEpoch).ToUniversalTime();
                    legalRep.TaxId = request.LegalRep.RFC ?? string.Empty;
                    legalRep.CURP = request.LegalRep.CURP ?? string.Empty;

                    await legalRepRepository.UpdateAsync(legalRep);
                }

                await legalRepRepository.CommitAsync();
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
                                ClientName = c.BusinessName ?? c.FullName,
                                LegalRepName = c.LegalRepresentative != null ? c.LegalRepresentative.FullName : null,
                                HasCredit = c.ClientCreditAccount != null,
                                CreditAmount = c.ClientCreditAccount != null ? c.ClientCreditAccount.CreditLimit : null,
                                PendingAmount = c.Orders.Sum(x => x.Total)
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
                                ClientName = string.IsNullOrEmpty(c.BusinessName) ? c.FullName : c.BusinessName,
                                LegalRepName = c.LegalRepresentative != null ? c.LegalRepresentative.FullName : null,
                                HasCredit = c.ClientCreditAccount != null,
                                CreditAmount = c.ClientCreditAccount != null ? c.ClientCreditAccount.CreditLimit : null,
                                PendingAmount = c.Orders.Sum(x => x.Total)
                                                - (c.ClientCreditAccount == null ? 0
                                                : c.ClientCreditAccount.ClientCreditTransactions
                                                .Where(ct => ct.TransactionType == CreditTransactionType.Payment
                                                    || ct.TransactionType == CreditTransactionType.Reversal
                                                    || ct.TransactionType == CreditTransactionType.AdjustmentCredit)
                                                .Sum(x => x.Amount)),
                                AmountPaid = c.ClientCreditAccount != null ? c.ClientCreditAccount.ClientCreditTransactions.Where(t => t.TransactionType == Commons.Enums.CreditTransactionType.Payment).Sum(t => t.Amount) : null,
                                CreditStatus = c.ClientCreditAccount != null ? c.ClientCreditAccount.CreditStatus : null
                            });

            return query.ToListAsync();
        }

        public async Task<ClientDetailResult?> GetClientDetailById(long clientId)
        {
            ClientDetailResult? result = await repository.Get()
                .AsNoTracking()
                .Where(c => c.Id == clientId)
                .Select(c => new ClientDetailResult
                {
                    Id = c.Id,
                    ClientType = c.ClientType,
                    ClientName = c.FullName,
                    BusinessName = c.BusinessName,
                    Email = c.Email,
                    CountryPhoneCode = c.CountryPhoneCode,
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
                        PendingAmount = c.Orders.Sum(x => x.Total)
                                        - (c.ClientCreditAccount == null ? 0
                                        : c.ClientCreditAccount.ClientCreditTransactions
                                        .Where(ct => ct.TransactionType == CreditTransactionType.Payment
                                            || ct.TransactionType == CreditTransactionType.Reversal
                                            || ct.TransactionType == CreditTransactionType.AdjustmentCredit)
                                        .Sum(x => x.Amount)),
                        CreditStatus = c.ClientCreditAccount.CreditStatus,
                        AmountPaid = c.ClientCreditAccount.ClientCreditTransactions.Where(t => t.TransactionType == CreditTransactionType.Payment).Sum(t => t.Amount),
                        StartDate = c.ClientCreditAccount.StartDate,
                        EndDate = c.ClientCreditAccount.EndDate
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
                            || x.LegalRepName.ToLower().Contains(lowerSearchTerm));
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
