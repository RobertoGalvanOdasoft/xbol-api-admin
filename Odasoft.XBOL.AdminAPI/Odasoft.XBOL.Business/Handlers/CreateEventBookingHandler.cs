using Odasoft.XBOL.Business.Messages;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.DTO.Results;

namespace Odasoft.XBOL.Business.Handlers
{
    public class CreateEventBookingHandler
    {
        private readonly ITicketingClient _ticketingClient;
        private readonly EventScheduleService _eventScheduleService;
        private readonly SequenceTrackerService _sequenceTrackerService;

        private const string EVENT_ORDER_LOCALIZER_PREFIX = "ORD-E";

        public CreateEventBookingHandler(ITicketingClient ticketingClient, EventScheduleService eventScheduleService, SequenceTrackerService sequenceTrackerService)
        {
            _ticketingClient = ticketingClient;
            _eventScheduleService = eventScheduleService;
            _sequenceTrackerService = sequenceTrackerService;
        }

        public async Task<(BookingResult?, object?)> Handle(CreateEventBookingCommand command)
        {
            try
            {
                long? eventId = await _eventScheduleService.GetEventIdByExternalEventKeyAsync(command.Request.EventKey);

                if (eventId == null)
                {
                    Console.WriteLine($"Event with key {command.Request.EventKey} not found.");
                    return (null, null);
                }

                command.Request.Localizer = await _sequenceTrackerService.GenerateLocalizerAsync(EVENT_ORDER_LOCALIZER_PREFIX, eventId.Value);

                var tickets = await _ticketingClient.BookEventSeatsAsync(command.Request);

                return (new BookingResult
                {
                    Message = "Booking created successfully",
                    Tickets = tickets,
                    Localizer = command.Request.Localizer,
                    ClientEmail = command.Request.ClientContact.Email,
                    ClientPhone = command.Request.ClientContact.PhoneNumber
                }, new CreateEventOrderCommand(command.Request));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating event booking: {ex.Message}");
                return (null, null);
            }
        }
    }
}
