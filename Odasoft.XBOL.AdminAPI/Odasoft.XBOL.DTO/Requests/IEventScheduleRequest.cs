using Odasoft.XBOL.Commons.Enums;

namespace Odasoft.XBOL.DTO.Requests
{
    public interface IEventScheduleRequest
    {
        long EventId { get; set; }
        DateTimeOffset StartDateTime { get; set; }
        DateTimeOffset EndDateTime { get; set; }

        DateTimeOffset PublishedDate { get; set; }
        DateTimeOffset PreSaleDate { get; set; }
        DateTimeOffset PreSaleEndDate { get; set; }
        DateTimeOffset OnSaleDate { get; set; }
        DateTimeOffset OffSaleDate { get; set; }
        DateTimeOffset GateOpenDate { get; set; }
        AgeRestriction? AgeRestriction { get; set; }
        string ExternalEventKey { get; set; }
        string? SecurityPolicies { get; set; }
        string? AdditionalComments { get; set; }

    }
}
