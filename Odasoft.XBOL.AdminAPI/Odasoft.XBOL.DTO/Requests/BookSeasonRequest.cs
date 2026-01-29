using XBOL.Admin.Core.DTO;

namespace Odasoft.XBOL.DTO.Requests
{
    public class BookSeasonRequest
    {
        public required ClientContactRequest ClientContact { get; set; } = new ClientContactRequest();
        public required List<string> Seats { get; set; } = new List<string>();
        public required string HoldToken { get; set; }

        public required string EventKey { get; set; }
        public required long SeassonId { get; set; }
        public long? RelatedOrderId { get; set; }
    }
}
