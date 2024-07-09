using BookLibrary.Business.Abstractions;
using BookLibrary.Infrastructure.AppSettings;
using BookLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BookLibrary.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IAuthenticationService _authenticationService;
        

        public AuthenticationController(IAuthenticationService authenticationService, IOptions<AppSettings> appSettings,  ILogger<AuthenticationController> logger)
        {
            this._authenticationService = authenticationService;
            this._logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] AuthenticateRequest authenticateRequest)
        {
            _logger.LogInformation("Entering Login Post");
            var authenticatedUser = _authenticationService.GetAuthenticatedUser(authenticateRequest);

            if (authenticatedUser == null)
            {
                _logger.LogInformation("Unauthenticated Login");
                return Unauthorized();
            }
            _logger.LogInformation("Authenticated Login");

            return Ok(new
            {
                authenticatedUser.AccessToken,
                authenticatedUser.RefreshToken,
                authenticatedUser.Email,
                Role = authenticatedUser.Role.ToString(),
                authenticatedUser.DisplayName
            });
        }

        [AllowAnonymous]
        [HttpPost("UpdateUserLogin")]
        public async Task<IActionResult> UpdateLoginDetails([FromBody] UserAdd request)
        {
            _logger.LogInformation("Entering UpdateUserLogin Post");
            var result = await _authenticationService.UpdateUserLogin(request);
            _logger.LogInformation("Updated User Login Details");

            return Ok(result);
        }

        [HttpPost("RegisterUser")]
        // [Authorize(Roles = "Admin")]
        public async Task<ResponseMessage<UserDto>> CreateLogin(string email)
        {
            _logger.LogInformation("Entering CreateLogin Post");
            var result = await _authenticationService.CreateLogin(email);
            _logger.LogInformation("Added Login entry");

            return result;
        }
    }
}
