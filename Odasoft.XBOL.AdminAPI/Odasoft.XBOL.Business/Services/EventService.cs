using Microsoft.AspNetCore.Identity;
using Odasoft.XBOL.Commons.Enums;
using Odasoft.XBOL.Data.Repositories;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Business.Services
{
    public class EventService
    {
        private readonly EventRepository _eventRepository;
        private readonly UserManager<User> _userManager;
        private readonly OrderRepository _orderRepository;
        private readonly EventSeatRepository _eventSeatRepository;
        private readonly EventScheduleRepository _eventScheduleRepository;

        public EventService(EventRepository eventRepository,
            UserManager<User> userManager,
            OrderRepository orderRepository,
            EventSeatRepository eventSeatRepository,
            EventScheduleRepository eventScheduleRepository)
        //SeatsIoService _seatsIoService)
        {
            _eventRepository = eventRepository;
            _userManager = userManager;
            _orderRepository = orderRepository;
            _eventSeatRepository = eventSeatRepository;
            _eventScheduleRepository = eventScheduleRepository;
        }

        public async Task<Event?> GetEventByIdAsync(long eventId)
        {
            // TODO: Set the proper DTO to return only necessary fields
            var result = await _eventRepository.GetByIdAsync(eventId);

            // TODO: Handle null result (e.g., throw exception or return a default value)
            return result;
        }

        // TODO: move to OrderService
        public async Task BookSeatsAsync(BookingRequest request)
        {
            //ChangeObjectStatusResult result = await _seatsIoService.BookSeatsAsync(request);

            // TODO: Get seller from Identity, and seller email from request, create custom NotFoundException
            User? buyer = await _userManager.FindByEmailAsync("admin@xbol.com") ?? throw new KeyNotFoundException();

            User? seller = buyer;

            EventSchedule schedule = _eventScheduleRepository.Get(x => x.ExternalEventKey == request.EventId).First();

            // TODO: Calculate total, taxes, and fees

            // Create Order
            var newOrder = new Order
            {
                UserId = buyer.Id,
                Reference = request.HoldToken,
                Status = OrderStatus.Pending,
                SubTotal = 0,
                TotalFees = 0,
                TotalTaxes = 0,
                Total = 0,
                OrderType = OrderType.Ticket,
                PayformType = PayformType.BoxOffice,

                CreatedAt = DateTimeOffset.Now,
                CreatedBy = seller.Id,
                UpdatedAt = DateTimeOffset.Now,
                UpdatedBy = seller.Id,
                Items = [.. request.Seats.Select(x => new OrderItem
                    {
                        ItemType = ItemType.Ticket,
                    // TODO: Check what is this field
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
