using Odasoft.XBOL.Commons.Email;

namespace Odasoft.XBOL.Commons.Requests;

// TODO: Move this to a proper namespace like Odasoft.XBOL.Commons.Email.Models

public class OrderEmailModel : EmailModelBase
{
    public required string EventTitle { get; set; }
    public required string EventImageUrl { get; set; }
    public required OrderDetailsInfo OrderDetails { get; set; }
    public required List<SeatInfo> Seats { get; set; }
    public required string GoogleWalletUrl { get; set; }
    public required string AppleWalletUrl { get; set; }
    public required List<string> EntryInstructions { get; set; }
    public string? PromoBannerImageUrl { get; set; }
    public string? PromoBannerLinkUrl { get; set; }
    public EmailTheme Theme { get; set; } = new();
}

public class OrderDetailsInfo
{
    public required string OrderNumber { get; set; }
    public required string Date { get; set; }
    public required string Time { get; set; }
    public required VenueInfo Venue { get; set; }
}

public class VenueInfo
{
    public required string Name { get; set; }
    public required string Address { get; set; }
}

public class SeatInfo
{
    public required string SeatKey { get; set; }
    public required string Zone { get; set; }
    public required string Row { get; set; }
    public required string Seat { get; set; }
}
