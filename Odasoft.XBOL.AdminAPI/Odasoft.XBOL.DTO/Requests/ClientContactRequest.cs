namespace XBOL.Admin.Core.DTO
{
    public class ClientContactRequest
    {
        public string CountryPhoneISO { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string LastName { get; set; } = null!;
    }
}
