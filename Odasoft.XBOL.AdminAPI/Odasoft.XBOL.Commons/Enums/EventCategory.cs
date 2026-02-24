using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Odasoft.XBOL.Commons.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EventCategory
    {
        [Description("Sports")]
        Sports,

        [Description("Concert")]
        Concert,

        [Description("Theater")]
        Theater
    }
}
