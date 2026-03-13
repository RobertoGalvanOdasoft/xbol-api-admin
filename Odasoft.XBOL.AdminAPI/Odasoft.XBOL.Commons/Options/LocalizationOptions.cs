using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Odasoft.XBOL.Commons.Options;

public class LocalizationOptions
{
    [Required]
    [MinLength(1)]
    [Description("List of supported culture codes (e.g. es-MX, en)")]
    public string[] SupportedCultures { get; set; } = ["es-MX", "en"];

    [Required]
    [Description("Default culture code used when no Accept-Language header is provided")]
    public string DefaultCulture { get; set; } = "es-MX";
}
