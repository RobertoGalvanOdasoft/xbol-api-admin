using Odasoft.XBOL.Commons.Enums;

namespace Odasoft.XBOL.DTO;

public class SeatManagementDetailDTO
{
    public required BookableUnitType Type { get; set; }
    public required string ExternalKey { get; set; }
    public required string Name { get; set; }
    public string? Subtitle { get; set; }
    public string? BannerImageUrl { get; set; }
    public string? VenueName { get; set; }
    public required DateTimeOffset StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public required int TotalSeats { get; set; }
    public required int AvailableSeats { get; set; }
    public IEnumerable<SeatsIoPriceDTO> Prices { get; set; } = [];
}
