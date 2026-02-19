using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Odasoft.XBOL.Commons.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum VenueCategory
    {
        [Description("Stadium")]
        Stadium,

        [Description("Arena")]
        Arena,

        [Description("Theater")]
        Theater,

        [Description("Club")]
        Club
    }
}
