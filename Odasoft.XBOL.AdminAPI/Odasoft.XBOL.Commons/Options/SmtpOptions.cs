using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Odasoft.XBOL.Commons.Options;

public class SmtpOptions
{
    [Required]
    [MinLength(1)]
    [Description("SMTP server hostname (e.g., smtp.sendgrid.net or localhost for smtp4dev)")]
    public string Host { get; set; } = "";

    [Range(1, 65535)]
    [DefaultValue(587)]
    [Description("SMTP server port (25 = plain, 587 = STARTTLS, 465 = implicit TLS)")]
    public int Port { get; set; } = 587;

    [Description("SMTP username for authentication (leave empty for unauthenticated)")]
    public string? Username { get; set; }

    [Description("SMTP password for authentication")]
    public string? Password { get; set; }

    [DefaultValue(false)]
    [Description("Use implicit TLS (port 465). When false, STARTTLS is used if available")]
    public bool UseSsl { get; set; }

    [Required]
    [EmailAddress]
    [Description("Default sender email address (From header)")]
    public string FromAddress { get; set; } = "";

    [Required]
    [MinLength(1)]
    [Description("Default sender display name (From header)")]
    public string FromName { get; set; } = "";
}
