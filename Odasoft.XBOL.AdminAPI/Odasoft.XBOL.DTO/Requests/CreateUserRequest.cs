namespace Odasoft.XBOL.DTO.Requests
{
    public class CreateUserRequest
    {
        // TODO: Add validation attributes as needed

        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
