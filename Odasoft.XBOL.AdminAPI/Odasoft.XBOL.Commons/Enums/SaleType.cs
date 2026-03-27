using System.Text.Json.Serialization;

namespace Odasoft.XBOL.Commons.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SaleType
    {
        Event,
        SeasonPass,
        EventGroup
    }
}
