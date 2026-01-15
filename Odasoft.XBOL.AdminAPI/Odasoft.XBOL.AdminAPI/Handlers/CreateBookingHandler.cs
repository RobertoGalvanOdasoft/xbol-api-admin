using Odasoft.XBOL.AdminAPI;
using Odasoft.XBOL.DTO.Results;

public class CreateBookingHandler
{
    private readonly ITicketingClient _ticketingClient;

    public CreateBookingHandler(ITicketingClient ticketingClient)
    {
        _ticketingClient = ticketingClient;
    }

    public async Task<BookingResult> Handle(CreateBookingCommand command)
    {
        var tickets = await _ticketingClient.BookingAsync(command.Request);
        return new BookingResult { Message = "Booking created successfully", Tickets = tickets };
    }
}