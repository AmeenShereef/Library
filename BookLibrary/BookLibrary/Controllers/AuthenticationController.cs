using BookLibrary.Business.Abstractions;
using BookLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BookLibrary.API.Controllers
{
    /// <summary>
    /// Controller responsible for authentication operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]    
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IAuthenticationService _authenticationService;

        /// <summary>
        /// Initializes a new instance of the AuthenticationController.
        /// </summary>
        /// <param name="authenticationService">The authentication service.</param>
        /// <param name="logger">The logger.</param>
        public AuthenticationController(IAuthenticationService authenticationService, ILogger<AuthenticationController> logger)
        {
            _authenticationService = authenticationService;
            _logger = logger;
        }

        /// <summary>
        /// Authenticates a user based on the provided credentials.
        /// </summary>
        /// <param name="authenticateRequest">The credentials to authenticate.</param>
        /// <returns>An IActionResult representing the authentication result.</returns>       
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
        [HttpPost("UpdateUserLogin")]
        public async Task<IActionResult> UpdateLoginDetails([FromBody] UserAdd request)
        {
            _logger.LogInformation("Entering UpdateUserLogin Post");
            var result = await _authenticationService.UpdateUserLogin(request);
            _logger.LogInformation("Updated User Login Details");

            return Ok(result);
        }

       

        /// <summary>
        /// Initiates the forgot password process for a user.
        /// </summary>
        /// <param name="email">The email address of the user requesting a password reset.</param>
        /// <returns>An IActionResult containing the result of the forgot password request.</returns>
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            _logger.LogInformation("Entering ForgetPassword Post");
            var result = await _authenticationService.ForgotPassword(email);
            _logger.LogInformation(result.Message);

            return Ok("Reset mail has been sent to the corresponding email.");
        }

        /// <summary>
        /// Updates the password for a user using a reset token.
        /// </summary>
        /// <param name="token">The reset token.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>An IActionResult containing the result of the password update.</returns>
        [HttpPost("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword(string token, string newPassword)
        {
            _logger.LogInformation("Entering UpdatePassword Post");
            var result = await _authenticationService.UpdatePassword(token, newPassword);
            _logger.LogInformation(result.Message);

            return Ok(result);
        }
    }
}