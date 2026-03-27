using Hangfire;
using Odasoft.XBOL.Business.Messages;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.Commons.Email;

namespace Odasoft.XBOL.Business.Handlers
{
    public class SendOrderCompletedNotificationHandler(OrderService orderService, IBackgroundJobClient backgroundJobClient)
    {
        public async Task Handle(SendOrderCompletedNotificationCommand message)
        {
            // TODO: Get client email and name from the order details instead of passing them in the command
            var model = await orderService.BuildOrderConfirmationAsync(message.OrderId, message.ToEmail, message.ToName, message.Culture);
            var jobId = backgroundJobClient.Enqueue<IEmailJob>(x => x.SendOrderConfirmationAsync(model));
        }
    }
}
