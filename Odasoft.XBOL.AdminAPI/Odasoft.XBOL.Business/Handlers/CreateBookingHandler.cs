
using Odasoft.XBOL.Business;
using Odasoft.XBOL.Business.Messages;
using Odasoft.XBOL.DTO.Results;

public class CreateBookingHandler
{
    private readonly ITicketingClient _ticketingClient;

    public CreateBookingHandler(ITicketingClient ticketingClient)
    {
        _ticketingClient = ticketingClient;
    }

    public async Task<(BookingResult, object)> Handle(CreateBookingCommand command)
    {
        var tickets = await _ticketingClient.BookSeatsAsync(command.Request);

        // TODO: In case of booking failure return null instead of CreateOrderCommand so the order is not created.
        return (new BookingResult { Message = "Booking created successfully", Tickets = tickets }, new CreateOrderCommand(command.Request));
    }
}
