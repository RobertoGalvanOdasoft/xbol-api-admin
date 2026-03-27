using Fluid;
using Fluid.Values;
using Microsoft.Extensions.Localization;

namespace Odasoft.XBOL.Commons.Email;

public static class LocalizationFilters
{
    public const string LocalizerKey = "StringLocalizer";

    /// <summary>
    /// Simple key lookup: {{ "Key" | t }}
    /// </summary>
    public static ValueTask<FluidValue> Translate(
        FluidValue input, FilterArguments _, TemplateContext context)
    {
        var localizer = (IStringLocalizer)context.AmbientValues[LocalizerKey];
        var key = input.ToStringValue();
        return new ValueTask<FluidValue>(new StringValue(localizer[key].Value));
    }

    /// <summary>
    /// Key lookup with positional args: {{ "Key" | t_format: arg1, arg2 }}
    /// </summary>
    public static ValueTask<FluidValue> TranslateFormat(
        FluidValue input, FilterArguments arguments, TemplateContext context)
    {
        var localizer = (IStringLocalizer)context.AmbientValues[LocalizerKey];
        var key = input.ToStringValue();
        var args = new object[arguments.Count];
        for (var i = 0; i < arguments.Count; i++)
        {
            args[i] = arguments.At(i).ToStringValue();
        }

        return new ValueTask<FluidValue>(new StringValue(localizer[key, args].Value));
    }
}
