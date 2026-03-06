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
    }
}
