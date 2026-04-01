using Odasoft.XBOL.Commons.Enums;

namespace Odasoft.XBOL.DTO.Requests
{
    public class OrderActionRequest
    {
        public required Dictionary<long, string> Seats { get; set; }

        public required OrderAction Action { get; set; }

        public required string ActionName { get; set; }

        public required string Comments { get; set; }

        public required bool ChangeToDigital { get; set; }

        public required string NewEmail { get; set; }
    }
}
