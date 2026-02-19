using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Commons.Constants;
using Odasoft.XBOL.Commons.Enums;
using Odasoft.XBOL.Commons.Requests.Filters;
using Odasoft.XBOL.Commons.Responses;
using Odasoft.XBOL.Data.Repositories;
using Odasoft.XBOL.Data.Repositories.Order;
using Odasoft.XBOL.Data.Repositories.Season;
using Odasoft.XBOL.DTO.QueryParams;
using Odasoft.XBOL.DTO.Requests;
using Odasoft.XBOL.DTO.Results;
using Odasoft.XBOL.Models;
using XBOL.Admin.Core.DTO;

namespace Odasoft.XBOL.Business.Services
{
    public class OrderService
    {
        private readonly OrderRepository _orderRepository;
        private readonly EventScheduleRepository _eventScheduleRepository;
        private readonly TicketRepository _ticketRepository;
        private readonly SeasonPassRepository _seasonPassRepository;

        public OrderService(OrderRepository orderRepository,
            EventScheduleRepository eventScheduleRepository,
            TicketRepository ticketRepository,
            SeasonPassRepository seasonPassRepository)
        {
            _orderRepository = orderRepository;
            _eventScheduleRepository = eventScheduleRepository;
            _ticketRepository = ticketRepository;
            _seasonPassRepository = seasonPassRepository;
        }

        public async Task CreateOrderAsync(BookingRequest request)
        {
            // TODO: Get seller from Identity, and buyer email from request, create custom NotFoundException

            EventSchedule schedule = _eventScheduleRepository.Get(x => x.ExternalEventKey == request.EventId).First();

            // TODO: Calculate total, taxes, and fees

            // Create Order
            var newOrder = new Order
            {
                UserId = null,
                //TODO: Check the correct value of reference
                Reference = request.HoldToken,
                Status = OrderStatus.Pending,
                SubTotal = 0,
                TotalFees = 0,
                TotalTaxes = 0,
                Total = 0,
                // TODO: Get OrderType and ItemType from request
                OrderType = OrderType.Ticket,
                PayformType = PayformType.BoxOffice,

                CreatedAt = DateTime.UtcNow,
                CreatedBy = Guid.Empty,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = Guid.Empty,
                Items = [.. request.Seats.Select(x => new OrderItem
                    {
                        ItemType = ItemType.Ticket,
                    // TODO: Insert reference Id, for this we need to either generate tickets first or update this field after payment confirmation for that we need another field to find the relationship with the ticket like the seat, we could also generate items and tickets after payment confirmation but that doesn't make much sense since Items it's part of an order that could be cancelled
                        ItemReferenceId = 0,
                        Price = 0
                    })]
            };

            // TODO: Confirm if Ticket creation should be done after payment confirmation

            await _orderRepository.InsertAsync(newOrder);
            await _orderRepository.CommitAsync();

            // Process Payment
        }

        public async Task<PagedResponse<OrderListItem>> GetOrderListAsync(OrderListFilters filters)
        {
            filters.Page = Math.Max(filters.Page, 1);
            filters.PageSize = Math.Clamp(filters.PageSize, 1, 50);

            (List<OrderListItem> result, int totalCount) = await _orderRepository.GetOrderListAsync(filters);

            return new PagedResponse<OrderListItem>
            {
                Items = result,
                CurrentPage = filters.Page,
                PageSize = filters.PageSize,
                TotalItems = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)filters.PageSize)
            };
        }

        public async Task<ClientSeasonEvent> GetClientSeasonEventByOrderReferenceAsync(string orderReference)
        {
            return await _orderRepository.GetClientSeasonEventByOrderReferenceAsync(orderReference);
        }

        public async Task BookSeasonAsync(BookSeasonRequest request)
        {
            await _orderRepository.BookSeasonAsync(request);
        }

        public async Task<DTO.Response.PagedResponse<OrderResult>> GetOrdersAsync(OrdersQueryParams queryParams)
        {
            IQueryable<Order> orders = queryParams.ClientId.HasValue
                                        ? _orderRepository.Get().AsNoTracking().Where(x => x.ClientId == queryParams.ClientId)
                                        : _orderRepository.Get().AsNoTracking();

            IQueryable<Ticket> tickets = _ticketRepository.Get().AsNoTracking();
            IQueryable<SeasonPass> seasonPasses = _seasonPassRepository.Get().AsNoTracking();

            var query = orders.Select(o => new OrderResult
            {
                Id = o.Id,
                OrderDate = o.CreatedAt,
                NumberOfItems = o.Items.Count,
                Amount = o.Total, // Assuming Total is the Amount you want

                // We look at the first item in the order, check its type,
                // and query the respective IQueryable to get the event name.
                Event = o.Items.OrderBy(i => i.Id) // Optional: Ensures we consistently get the "first" item
                        .Select(i => i.ItemType == ItemType.Ticket
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
