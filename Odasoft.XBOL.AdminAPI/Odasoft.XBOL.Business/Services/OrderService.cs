using Microsoft.AspNetCore.Identity;
using Odasoft.XBOL.Commons.Enums;
using Odasoft.XBOL.Data.Repositories;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Business.Services
{
    public class OrderService(
        OrderRepository _orderRepository,
        UserManager<User> _userManager,
        EventScheduleRepository _eventScheduleRepository
        )
    {
        public async Task CreateOrderAsync(BookingRequest request)
        {
            // TODO: Get seller from Identity, and buyer email from request, create custom NotFoundException
            User? buyer = await _userManager.FindByEmailAsync("admin@xbol.com") ?? throw new KeyNotFoundException();

            User? seller = buyer;

            EventSchedule schedule = _eventScheduleRepository.Get(x => x.ExternalEventKey == request.EventId).First();

            // TODO: Calculate total, taxes, and fees

            // Create Order
            var newOrder = new Order
            {
                UserId = buyer.Id,
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
                CreatedBy = seller.Id,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = seller.Id,
                Items = [.. request.Seats.Select(x => new OrderItem
                    {
                        ItemType = ItemType.Ticket,
                    // TODO: Insert reference Id, for this we need to either generate tickets first or update this field after payment confirmation for that we need another field to find the relationship with the ticket like the seat, we could also generate items and tickets after payment confirmation but that doesn't make much sense since Items it's part of an order that could be cancelled
                        ItemReferenceId = 0,
                        Price = 0
                    })]
            };

            // TODO: Confirm if Ticket creation should be done after payment confirmation
            //foreach (var item in result.Objects)
            //{
            //    // TODO: Get all seats info and handle in memory to avoid multiple calls to database
            //    EventSeat seat = await _eventSeatRepository.GetByExternalSeatObjectKeyAsync(item.Key) ?? throw new KeyNotFoundException();

            //    newOrder.Tickets.Add(new Ticket
            //    {
            //        EventSeatId = seat.Id,
            //        Status = TicketStatus.Issued,
            //        EventScheduleId = schedule.Id,
            //        EventSectionId = seat.EventSectionId,
            //    });
            //}

            await _orderRepository.InsertAsync(newOrder);
            await _orderRepository.CommitAsync();

            // Process Payment
        }
    }
}
