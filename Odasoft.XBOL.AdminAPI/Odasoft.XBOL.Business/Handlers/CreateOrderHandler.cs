using Odasoft.XBOL.Business.Messages;
using Odasoft.XBOL.Business.Services;

namespace Odasoft.XBOL.Business.Handlers
{
    public class CreateOrderHandler(OrderService orderService)
    {
        public async Task Handle(CreateOrderCommand message)
        {
            await orderService.CreateOrderAsync(message.Request);
        }
    }
}
