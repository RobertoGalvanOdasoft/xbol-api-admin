using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Odasoft.XBOL.Commons.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SeatType
    {
        [Description("Stadium")]
        Standard,

        [Description("Accessible")]
        Accessible,

        [Description("Vip")]
        Vip
    }
}
