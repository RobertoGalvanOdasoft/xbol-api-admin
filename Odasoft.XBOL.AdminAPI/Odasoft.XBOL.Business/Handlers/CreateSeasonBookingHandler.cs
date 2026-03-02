using Odasoft.XBOL.Business.Messages;
using Odasoft.XBOL.DTO.Results;

namespace Odasoft.XBOL.Business.Handlers
{
    public class CreateSeasonBookingHandler
    {
        private readonly ITicketingClient _ticketingClient;

        public CreateSeasonBookingHandler(ITicketingClient ticketingClient)
        {
            _ticketingClient = ticketingClient;
        }

        public async Task<(BookingResult?, object?)> Handle(CreateSeasonBookingCommand command)
        {
            try
            {
                var tickets = await _ticketingClient.BookSeasonSeatsAsync(command.Request);

                return (new BookingResult { Message = "Booking created successfully", Tickets = tickets }, new CreateSeasonOrderCommand(command.Request));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating season booking: {ex.Message}");
                return (null, null);
            }
        }
    }
}
