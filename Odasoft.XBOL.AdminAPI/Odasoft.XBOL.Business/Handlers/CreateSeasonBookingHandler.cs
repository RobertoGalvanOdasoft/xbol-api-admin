using Odasoft.XBOL.Business.Messages;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.DTO.Results;

namespace Odasoft.XBOL.Business.Handlers
{
    public class CreateSeasonBookingHandler
    {
        private readonly ITicketingClient _ticketingClient;
        private readonly EventScheduleService _eventScheduleService;
        private readonly SeasonService _seasonService;
        private readonly SequenceTrackerService _sequenceTrackerService;

        private const string SEASON_ORDER_LOCALIZER_PREFIX = "ORD-S";

        public CreateSeasonBookingHandler(ITicketingClient ticketingClient, SeasonService seasonService, SequenceTrackerService sequenceTrackerService)
        {
            _ticketingClient = ticketingClient;
            _seasonService = seasonService;
            _sequenceTrackerService = sequenceTrackerService;
        }

        public async Task<(BookingResult?, object?)> Handle(CreateSeasonBookingCommand command)
        {
            try
            {
                long? seasonId = await _seasonService.GetSeasonIdByExternalKeyAsync(command.Request.SeasonKey);

                if (seasonId == null)
                {
                    Console.WriteLine($"Season with key {command.Request.SeasonKey} not found.");
                    return (null, null);
                }

                command.Request.Localizer = await _sequenceTrackerService.GenerateLocalizerAsync(SEASON_ORDER_LOCALIZER_PREFIX, seasonId.Value);

                var tickets = await _ticketingClient.BookSeasonSeatsAsync(command.Request);

                return (new BookingResult
                {
                    Message = "Booking created successfully",
                    Tickets = tickets,
                    Localizer = command.Request.Localizer,
                    ClientEmail = command.Request.ClientContact.Email,
                    ClientPhone = command.Request.ClientContact.PhoneNumber
                },
                new CreateSeasonOrderCommand(command.Request));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating season booking: {ex.Message}");
                return (null, null);
            }
        }
    }
}
