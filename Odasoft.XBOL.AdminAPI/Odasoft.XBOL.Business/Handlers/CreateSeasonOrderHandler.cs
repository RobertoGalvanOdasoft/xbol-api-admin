using Odasoft.XBOL.Business.Messages;
using Odasoft.XBOL.Business.Services;

namespace Odasoft.XBOL.Business.Handlers
{
    public class CreateSeasonOrderHandler(OrderService orderService)
    {
        public async Task Handle(CreateSeasonOrderCommand message)
        {
            await orderService.CreateSeasonOrderAsync(message.Request);
        }
    }
}
