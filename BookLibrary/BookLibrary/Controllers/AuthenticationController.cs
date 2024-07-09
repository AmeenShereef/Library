using BookLibrary.Business.Abstractions;
using BookLibrary.Business.Constants;
using BookLibrary.Infrastructure.AppSettings;
using BookLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace BookLibrary.API.Controllers
{
    /// <summary>
    /// Controller responsible for authentication operations.
    /// Requires authorization for all endpoints except for 'Login' and 'UpdateUserLogin'.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IAuthenticationService _authenticationService;
        /// <summary>
        /// Controller constructor
        /// </summary>
        public AuthenticationController(IAuthenticationService authenticationService, IOptions<AppSettings> appSettings, ILogger<AuthenticationController> logger)
        {
            this._authenticationService = authenticationService;
            this._logger = logger;
        }

        /// <summary>
        /// Authenticates a user based on the provided credentials.
        /// </summary>
        /// <param name="authenticateRequest">The credentials to authenticate.</param>
        /// <returns>An IActionResult representing the authentication result.</returns>
        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login([FromBody] AuthenticateRequest authenticateRequest)
        {
            _logger.LogInformation("Entering Login Post");
            var authenticatedUser = _authenticationService.AuthenticateUser(authenticateRequest);

            if (authenticatedUser == null)
            {
                _logger.LogInformation("Unauthenticated Login");
                return Unauthorized();
            }
            _logger.LogInformation("Authenticated Login");

            return Ok(authenticatedUser);
        }

        /// <summary>
        /// Updates the login details of a user.
        /// </summary>
        /// <param name="request">The details to update for the user.</param>
        /// <returns>An IActionResult representing the result of the update operation.</returns>
        [AllowAnonymous]
        [HttpPost("UpdateUserLogin")]
        public async Task<IActionResult> UpdateLoginDetails([FromBody] UserAdd request)
        {
            _logger.LogInformation("Entering UpdateUserLogin Post");
            var result = await _authenticationService.UpdateUserLogin(request);
            _logger.LogInformation("Updated User Login Details");

            return Ok(result);
        }

        /// <summary>
        /// Registers a new user with the specified email and role.
        /// Use 0 for admin and 1 for user.
        /// </summary>
        /// <param name="email">The email of the user to be registered.</param>
        /// <param name="role">The role of the user (0 for admin, 1 for user).</param>
        /// <returns>A response message containing the registered user details.</returns>
        [HttpPost("RegisterUser")]
        [Authorize(Roles = "Admin")]
        public async Task<ResponseMessage<UserDto>> RegisterUser(string email)
        {
            _logger.LogInformation("Entering CreateLogin Post");
            var result = await _authenticationService.RegisterUser(email);
            _logger.LogInformation("Added Login entry");

            return result;
        }
    }
}
