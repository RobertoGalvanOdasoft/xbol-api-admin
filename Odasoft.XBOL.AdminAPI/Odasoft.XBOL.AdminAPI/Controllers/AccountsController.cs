using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.DTO.Requests;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private UserManager<User> _userManager;

        public AccountsController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Registers a new user account using the specified registration details.
        /// </summary>
        /// <remarks>This method creates a new user account with the provided credentials. The
        /// registration may fail if the user name or email address is already in use, or if the password does not meet
        /// the required criteria. The response does not include detailed error information; for more granular feedback,
        /// consider extending the response model.</remarks>
        /// <param name="request">An object containing the user registration information, including user name, email address, and password.
        /// Cannot be null.</param>
        /// <returns>An ActionResult containing <see langword="true"/> if the user was registered successfully.</returns>
        [HttpPost]
        public async Task<ActionResult<bool>> RegisterUser(CreateUserRequest request)
        {
            User newUser = new()
            {
                UserName = request.UserName,
                Email = request.Email
            };

            IdentityResult result = await _userManager.CreateAsync(newUser, request.Password);

            return Ok(true);
        }
    }
}
