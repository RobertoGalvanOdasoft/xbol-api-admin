using Odasoft.XBOL.Business.Messages;
using Odasoft.XBOL.DTO.Results;

namespace Odasoft.XBOL.Business.Handlers
{
    public class CreateEventBookingHandler
    {
        private readonly ITicketingClient _ticketingClient;

        public CreateEventBookingHandler(ITicketingClient ticketingClient)
        {
            _ticketingClient = ticketingClient;
        }

        public async Task<(BookingResult?, object?)> Handle(CreateEventBookingCommand command)
        {
            try
            {
                var tickets = await _ticketingClient.BookEventSeatsAsync(command.Request);

                return (new BookingResult { Message = "Booking created successfully", Tickets = tickets }, new CreateEventOrderCommand(command.Request));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating event booking: {ex.Message}");
                return (null, null);
            }
        }
    }
}
