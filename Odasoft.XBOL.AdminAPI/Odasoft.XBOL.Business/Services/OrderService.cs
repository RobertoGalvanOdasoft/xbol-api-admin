using Microsoft.AspNetCore.Identity;
using Odasoft.XBOL.Commons.Enums;
using Odasoft.XBOL.Commons.Requests.Filters;
using Odasoft.XBOL.Commons.Responses;
using Odasoft.XBOL.Data.Repositories;
using Odasoft.XBOL.Data.Repositories.Order;
using Odasoft.XBOL.DTO.Requests;
using Odasoft.XBOL.Models;
using XBOL.Admin.Core.DTO;

namespace Odasoft.XBOL.Business.Services
{
    public class OrderService(
        OrderRepository _orderRepository,
        UserManager<User> _userManager,
        EventScheduleRepository _eventScheduleRepository)
    {
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
    }
}
