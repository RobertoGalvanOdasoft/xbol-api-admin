using Odasoft.XBOL.Commons.Requests;

namespace Odasoft.XBOL.Commons.Email;

public interface IEmailJob
{
    Task SendTestEmailAsync(TestEmailModel model);
    Task SendOrderConfirmationAsync(OrderConfirmationModel model);
}
