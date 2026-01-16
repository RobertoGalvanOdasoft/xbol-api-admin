using Odasoft.XBOL.Business;
using Odasoft.XBOL.DTO.Results;

public class CreateBookingHandler
{
    private readonly ITicketingApi _ticketingApi;

    public CreateBookingHandler(ITicketingApi ticketingApi)
    {
        _ticketingApi = ticketingApi;
    }

    public async Task<BookingResult> Handle(CreateBookingCommand command)
    {
        var tickets = await _ticketingApi.BookingAsync(command.Request);
        return new BookingResult { Message = "Booking created successfully", Tickets = tickets };
    }
}
