using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Odasoft.XBOL.Commons.Options;

public class FileUploadOptions
{
    [Range(1, long.MaxValue)]
    [DefaultValue(104857600L)]
    [Description("Maximum multipart body length in bytes (default: 100 MB)")]
    public long MultipartBodyLengthLimit { get; set; } = 104857600;

    [Required]
    [MinLength(1)]
    [DefaultValue(new[] { ".pdf", ".docx", ".doc", ".png", ".jpeg", ".jpg", ".webp" })]
    [Description("Allowed file extensions for uploads")]
    public string[] AllowedExtensions { get; set; } = [".pdf", ".docx", ".doc", ".png", ".jpeg", ".jpg", ".webp"];
}
