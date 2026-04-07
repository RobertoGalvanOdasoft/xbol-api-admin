using Odasoft.XBOL.Commons.Enums;

namespace Odasoft.XBOL.DTO.Responses
{
    public class OrderActionResponse
    {
        public required long OrderId { get; set; }

        public required string Seats { get; set; }

        public required OrderAction Action { get; set; }

        public required string ActionName { get; set; }

        public required string Comments { get; set; }

        public required DateTimeOffset CreatedAt { get; set; }

        public required string CreatedBy { get; set; }
    }
}
