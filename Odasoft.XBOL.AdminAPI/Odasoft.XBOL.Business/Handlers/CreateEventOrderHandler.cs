using Odasoft.XBOL.Business.Messages;
using Odasoft.XBOL.Business.Services;

namespace Odasoft.XBOL.Business.Handlers
{
    public class CreateEventOrderHandler(OrderService orderService)
    {
        public async IAsyncEnumerable<object> HandleAsync(CreateEventOrderCommand message)
        {
            var orderId = await orderService.CreateEventOrderAsync(message.Request);

            // TODO: Send notifications based on request Delivery type (print, digital) and user preferences or info: Mail, push notification, SMS

            // TODO: Get client contact info from service or response
            yield return new SendOrderCompletedNotificationCommand(orderId, message.Request?.ClientContact?.Email ?? string.Empty, message.Request?.ClientContact?.FullName ?? string.Empty, "es-MX");

            // TODO: Send Mail for Seller
            yield return new SendOrderCompletedNotificationCommand(orderId, "support@xbol.com", "Seller", "es-MX");
        }
    }
}
