using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Odasoft.XBOL.Commons.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OrderAction
    {
        [Description("Order created.")]
        OrderCreated,

        [Description("Order renewed.")]
        OrderRenewed,

        [Description("Cancel order.")]
        CancelOrder,

        [Description("Cancel without refund.")]
        CancelWithoutRefund,

        [Description("Resend receipt.")]
        ResendReceipt,

        [Description("Update order holder.")]
        UpdateOrderHolder,

        [Description("Reissue tickets.")]
        ReissueTickets,

        [Description("Send individual tickets")]
        SendIndiviualTickets,

        [Description("Resend courtesy tickets.")]
        ResendCourtesyTickets,

        [Description("Convert to digital/physical tickets.")]
        ConvertToDigitalPyshical,

        [Description("Cancel tickets.")]
        CancelTickets
    }
}
