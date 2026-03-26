namespace Odasoft.XBOL.Commons.Email;

/// <summary>
/// Email template theme derived from the MudBlazor front-end palette.
/// </summary>
public class EmailTheme
{
    public string Primary { get; set; } = "#d4a12f";
    public string PrimaryDarken { get; set; } = "#bd8817";
    public string PrimaryLighten { get; set; } = "#fff8e0";
    public string Secondary { get; set; } = "#556e79";
    public string SecondaryDarken { get; set; } = "#3a4950";
    public string SecondaryLighten { get; set; } = "#ebeef1";
    public string Background { get; set; } = "#FAFAFA";
    public string Surface { get; set; } = "#FFFFFF";
    public string Success { get; set; } = "#32bd52";
    public string Error { get; set; } = "#e84238";
    public string Warning { get; set; } = "#f78d26";
    public string Info { get; set; } = "#3a8bec";
    public string FontFamily { get; set; } = "'Open Sans', Helvetica, Arial, sans-serif";

    // White-label properties
    public string LogoPath { get; set; } = "Assets/logo/PWRTickets-color@3x.png";
    public string LogoUrl { get; set; } = "https://placehold.co/200x80/EEE/31343C?text=PWR+TICKET";
    public string HelpUrl { get; set; } = "https://example.com/help";
    public string SupportUrl { get; set; } = "https://example.com/contact";
    public string SupportEmail { get; set; } = "soporte@pwrticket.com.mx";
}
