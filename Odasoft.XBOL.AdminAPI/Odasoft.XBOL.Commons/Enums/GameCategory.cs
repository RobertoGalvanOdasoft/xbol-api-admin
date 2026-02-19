using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Odasoft.XBOL.Commons.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GameCategory
    {
        [Description("Regular")]
        Regular,

        [Description("Playoff")]
        Playoff
    }
}
