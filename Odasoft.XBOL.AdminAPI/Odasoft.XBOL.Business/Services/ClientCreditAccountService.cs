using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Commons.Constants;
using Odasoft.XBOL.Data.Repositories;
using Odasoft.XBOL.Data.Repositories.Order;
using Odasoft.XBOL.Data.Repositories.Season;
using Odasoft.XBOL.DTO.QueryParams;
using Odasoft.XBOL.DTO.Requests;
using Odasoft.XBOL.DTO.Results;
using Odasoft.XBOL.Models;
using Enums = Odasoft.XBOL.Commons.Enums;

namespace Odasoft.XBOL.Business.Services
{
    public class ClientCreditAccountService
    {
        private readonly ClientCreditAccountRepository _clientCreditAccountRepository;
        private readonly ClientCreditTransactionRepository _clientCreditTransactionRepository;
        private readonly OrderRepository _orderRepository;
        private readonly SeasonPassRepository _seasonPassRepository;
        private readonly TicketRepository _ticketRepository;

        public ClientCreditAccountService(ClientCreditAccountRepository clientCreditAccountRepository,
            ClientCreditTransactionRepository clientCreditTransactionRepository,
            OrderRepository orderRepository,
            SeasonPassRepository seasonPassRepository,
            TicketRepository ticketRepository)
        {
            _clientCreditAccountRepository = clientCreditAccountRepository;
            _clientCreditTransactionRepository = clientCreditTransactionRepository;
            _orderRepository = orderRepository;
            _seasonPassRepository = seasonPassRepository;
            _ticketRepository = ticketRepository;
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

        public async Task<DTO.Response.PagedResponse<OrderResult>> GetCreditOrdersAsync(OrdersQueryParams queryParams)
        {
            IQueryable<ClientCreditTransaction> transactions = _clientCreditTransactionRepository
                                        .Get()
                                        .AsNoTracking()
                                        .Where(x => x.ClientCreditAccount.ClientId == queryParams.ClientId
                                            && x.TransactionType == Enums.CreditTransactionType.Drawdown);

            List<string> orderReferences = transactions.Select(x => x.Description).ToList();

            IQueryable<Order> orders = _orderRepository
                                        .Get()
                                        .AsNoTracking()
                                        .Where(x => orderReferences.Contains(x.Reference));

            IQueryable<Ticket> tickets = _ticketRepository.Get().AsNoTracking();
            IQueryable<SeasonPass> seasonPasses = _seasonPassRepository.Get().AsNoTracking();

            var query = orders.Select(o => new OrderResult
            {
                Id = o.Id,
                OrderDate = o.CreatedAt,
                NumberOfItems = o.Items.Count,
                Amount = transactions.Where(x => x.ReferenceId == o.Reference).First().Amount,

                // We look at the first item in the order, check its type,
                // and query the respective IQueryable to get the event name.
                Event = o.Items.OrderBy(i => i.Id) // Optional: Ensures we consistently get the "first" item
                        .Select(i => i.ItemType == Enums.ItemType.Ticket
                        ? tickets
                            .Where(t => t.Id == i.ItemReferenceId)
                            .Select(t => t.EventSchedule.Event.Name)
                            .FirstOrDefault()
                        : seasonPasses
                            .Where(sp => sp.Id == i.ItemReferenceId)
                            .Select(sp => sp.Season.Name)
                            .FirstOrDefault()
                ).FirstOrDefault() ?? "No Event"
            });

            SetFilters(ref query, queryParams);
            SetSearchTermFilter(ref query, queryParams.SearchTerm);
            SetOrder(ref query, queryParams.SortBy, queryParams.Descending);

            List<OrderResult> result = await query.ToListAsync();

            int totalCount = result.Count();

            return new DTO.Response.PagedResponse<OrderResult>
            {
                Items = result
                        .Skip(queryParams.Page * queryParams.PageSize)
                        .Take(queryParams.PageSize)
                        .ToList(),
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };
        }

        private void SetFilters(ref IQueryable<OrderResult> query, OrdersQueryParams queryParams)
        {
            if (queryParams.Events is not null && queryParams.Events.Any())
            {
                query = query.Where(x => queryParams.Events.Contains(x.Event));
            }

            // Filter between dates
            if (queryParams.StartDate.HasValue && queryParams.EndDate.HasValue)
            {
                var startDateUtc = queryParams.StartDate.Value.ToUniversalTime();
                var endDateUtc = queryParams.EndDate.Value.ToUniversalTime();

                query = query.Where(ct => ct.OrderDate >= startDateUtc && ct.OrderDate <= endDateUtc);
            }
        }

        private void SetSearchTermFilter(ref IQueryable<OrderResult> query, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm) == false)
            {
                var lowerSearchTerm = searchTerm.ToLower();
                query = query.Where(x => x.Event.ToLower().Contains(lowerSearchTerm));
            }
        }

        private void SetOrder(ref IQueryable<OrderResult> query, string sortBy, bool descending)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return;
            }

            // TODO: validate each case
            query = sortBy.ToLower() switch
            {
                QueryParamsFieldNames.ORDER_AMOUNT => descending ? query.OrderByDescending(x => x.Amount) : query.OrderBy(x => x.Amount),
                QueryParamsFieldNames.ORDER_EVENT => descending ? query.OrderByDescending(x => x.Event) : query.OrderBy(x => x.Event),
                QueryParamsFieldNames.ORDER_DATE => descending ? query.OrderByDescending(x => x.OrderDate) : query.OrderBy(x => x.OrderDate),
                QueryParamsFieldNames.ORDER_NUMBER_OF_ITEMS => descending ? query.OrderByDescending(x => x.NumberOfItems) : query.OrderBy(x => x.NumberOfItems),
                _ => query
            };
        }
    }
}
