using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Data.Repositories;
using Odasoft.XBOL.DTO.Requests;
using Odasoft.XBOL.DTO.Results;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Business.Services
{
    public class ClientCreditAccountService
    {
        private readonly ClientCreditAccountRepository _clientCreditAccountRepository;

        public ClientCreditAccountService(ClientCreditAccountRepository clientCreditAccountRepository)
        {
            _clientCreditAccountRepository = clientCreditAccountRepository;
        }

        public async Task<CreditAccountResult?> GetCreditAccountByClientIdAsync(long clientId)
        {
            ClientCreditAccount? creditAccount = await _clientCreditAccountRepository.Get()
                                                        .AsNoTracking()
                                                        .Where(x => x.ClientId == clientId)
                                                        .SingleOrDefaultAsync();

            if (creditAccount == null)
            {
                return null;
            }

            return new CreditAccountResult
            {
                Id = creditAccount.Id,
                ClientId = creditAccount.ClientId,
                CreditLimit = creditAccount.CreditLimit,
                StartDate = creditAccount.StartDate,
                PaymentFrequency = creditAccount.PaymentFrequency,
                CreditStatus = creditAccount.CreditStatus,
                AmountPaid = 0, // Calculate field base on transactions
                PendingAmount = creditAccount.CurrentBalance
            };
        }

        public async Task<bool> CreateClientCreditAccountAsync(CreateClientCreditAccountRequest request)
        {
            var newCreditAccount = new ClientCreditAccount
            {
                AppliesInterestRate = request.AppliesInterestRate,
                ClientId = request.ClientId,
                CreditLimit = request.CreditLimit,
                CreditStatus = Commons.Enums.CreditStatus.Pending,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow.ToUniversalTime(),
                CreatedBy = Guid.Empty,
                UpdatedAt = DateTimeOffset.UtcNow.ToUniversalTime(),
                UpdatedBy = Guid.Empty,
            };

            try
            {
                await _clientCreditAccountRepository.InsertAsync(newCreditAccount);
                await _clientCreditAccountRepository.CommitAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating credit account for the client. Error: {ex.Message}");
                return false;
            }
            return true;
        }

        public async Task<bool> UpdateClientCreditAccountByIdAsync(long clientCreditAccountId, UpdateClientCreditAccountRequest request)
        {
            ClientCreditAccount? existingCreditAccount = await _clientCreditAccountRepository.GetByIdAsync(clientCreditAccountId);

            if (existingCreditAccount == null)
            {
                Console.WriteLine($"Credit account with ID {clientCreditAccountId} not found.");
                return false;
            }

            existingCreditAccount.AppliesInterestRate = request.AppliesInterestRate;
            existingCreditAccount.CreditLimit = request.CreditLimit;
            existingCreditAccount.PaymentFrequency = request.PaymentFrequency;
            existingCreditAccount.StartDate = request.StartDate;
            existingCreditAccount.EndDate = request.EndDate;
            existingCreditAccount.UpdatedAt = DateTimeOffset.UtcNow.ToUniversalTime();
            existingCreditAccount.UpdatedBy = Guid.Empty;

            try
            {
                await _clientCreditAccountRepository.UpdateAsync(existingCreditAccount);
                await _clientCreditAccountRepository.CommitAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating Credit Account. Error: {ex.Message}");
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteClientCreditAccountByIdAsync(long creditAccountId)
        {
            ClientCreditAccount? existingCreditAccount = await _clientCreditAccountRepository.GetByIdAsync(creditAccountId);

            if (existingCreditAccount == null)
            {
                Console.WriteLine($"Credit account with ID {creditAccountId} not found.");
                return false;
            }

            try
            {
                await _clientCreditAccountRepository.HardDeleteAsync(existingCreditAccount);
                await _clientCreditAccountRepository.CommitAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting Credit Account. Error: {ex.Message}");
                return false;
            }

            return true;
        }
    }
}
