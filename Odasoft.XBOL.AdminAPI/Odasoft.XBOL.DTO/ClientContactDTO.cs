using Odasoft.XBOL.Commons.Enums;

namespace Odasoft.XBOL.DTO
{
    public class ClientContactDTO
    {
        public long Id { get; set; }
        public long? PhoneRegionCodeId { get; set; }
        public string PhoneNumber { get; set; } = "";
        public string Email { get; set; } = "";
        public string Name { get; set; } = "";
        public string LastName { get; set; } = "";
        public string? City { get; set; }
        public string? Neighbourhood { get; set; }
        public Gender? Gender { get; set; }
        public DateTimeOffset? DOB { get; set; }
    }
}
