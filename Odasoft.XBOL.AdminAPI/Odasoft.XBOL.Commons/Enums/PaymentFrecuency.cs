using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Odasoft.XBOL.Commons.Enums
{
    /// <summary>
    /// Common payment frequencies for mortage / loans
    /// </summary>
    /// <remarks>
    /// Monthly: 12 payments per year
    /// Bi-weekly: 26 payments per year (accelerated payment)
    /// Semi-monthly: 24 payments per year
    /// Weekly: 52 payments per year
    /// </remarks>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PaymentFrequency
    {
        [Description("Monthly")]
        Monthly,

        [Description("Bi-weekly")]
        BiWeekly,

        [Description("Semi-weekly")]
        SemiMonthly,

        [Description("Weekly")]
        Weekly
    }
}
