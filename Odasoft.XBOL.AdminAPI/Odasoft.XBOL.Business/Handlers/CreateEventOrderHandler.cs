using Odasoft.XBOL.Business.Messages;
using Odasoft.XBOL.Business.Services;

namespace Odasoft.XBOL.Business.Handlers
{
    public class CreateEventOrderHandler(OrderService orderService)
    {
        public async Task Handle(CreateEventOrderCommand message)
        {
            await orderService.CreateEventOrderAsync(message.Request);
        }
    }
}
