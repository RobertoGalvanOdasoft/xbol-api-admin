using Odasoft.XBOL.Commons.Enums;

namespace Odasoft.XBOL.DTO.Requests
{
    public class UpdateEventScheduleRequest : IEventScheduleRequest
    {
        public long Id { get; set; }
        public long EventId { get; set; }
        public DateTimeOffset StartDateTime { get; set; }
        public DateTimeOffset EndDateTime { get; set; }

        public DateTimeOffset PublishedDate { get; set; }
        public DateTimeOffset PreSaleDate { get; set; }
        public DateTimeOffset PreSaleEndDate { get; set; }
        public DateTimeOffset OnSaleDate { get; set; }
        public DateTimeOffset OffSaleDate { get; set; }
        public DateTimeOffset GateOpenDate { get; set; }
        public AgeRestriction? AgeRestriction { get; set; }
        public string? SecurityPolicies { get; set; }
        public string? AdditionalComments { get; set; }
    }
}
