using Odasoft.XBOL.Commons.Enums;

namespace Odasoft.XBOL.DTO;

public class EventScheduleDTO
{
    public required long Id { get; set; }
    public required DateTimeOffset StartDateTime { get; set; }
    public DateTimeOffset? EndDateTime { get; set; }
    public DateTimeOffset PublishedDate { get; set; }
    public DateTimeOffset PreSaleStartDate { get; set; }
    public DateTimeOffset PreSaleEndDate { get; set; }
    public DateTimeOffset OnSaleDate { get; set; }
    public DateTimeOffset OffSaleDate { get; set; }
    public DateTimeOffset GateOpenDate { get; set; }
    public required string ExternalEventKey { get; set; }
    public required int TotalSeats { get; set; }
    public required int AvailableSeats { get; set; }
    public required ScheduleStatus Status { get; set; }
}
