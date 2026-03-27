namespace Odasoft.XBOL.Business.Messages
{
    public record SendOrderCompletedNotificationCommand(long OrderId, string ToEmail, string ToName, string Culture)
    {
    }
}
