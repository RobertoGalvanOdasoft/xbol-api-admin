using Microsoft.Extensions.Localization;

namespace Odasoft.XBOL.AdminAPI.Helpers
{
    public static class EnumLocalizationHelper
    {
        public static string GetLocalizedEnum(Enum value, IStringLocalizer localizer)
        {
            var key = value.ToString();
            return localizer[key] ?? key;
        }
    }
}
