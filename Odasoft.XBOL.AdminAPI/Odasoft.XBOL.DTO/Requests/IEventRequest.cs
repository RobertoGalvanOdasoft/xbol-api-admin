using Odasoft.XBOL.Commons.Enums;

namespace Odasoft.XBOL.DTO.Requests
{
    public interface IEventRequest
    {
        long? VenueMapId { get; set; }
        string Name { get; set; }
        string? Subtitle { get; set; }
        string? ShortDescription { get; set; }
        string? LongDescription { get; set; }
        AgeRestriction? AgeRestriction { get; set; }
        string? SecurityPolicies { get; set; }
        string? AdditionalComments { get; set; }

        List<long> CategoryIds { get; set; }
    }
}
