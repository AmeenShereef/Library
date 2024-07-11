using BookLibrary.Business.Abstractions;
using BookLibrary.Business.AutenticateAggregate;
using BookLibrary.Business.Constants;
using BookLibrary.Business.Extensions;
using BookLibrary.Business.Services.PasswordHasher;
using BookLibrary.Data.Entities;
using BookLibrary.Infrastructure.AppSettings;
using BookLibrary.Models;
using BookLibrary.Repositories.Abstractions;
using Mapster;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookLibrary.Business.AuthenticateAggregate
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly AppSettings _appSettings;
        private readonly AuthenticationSettings _authenticationSettings;

        public AuthenticationService(
            IAuthenticationRepository authenticationRepository,
            IOptions<AppSettings> appSettings,
            IOptions<AuthenticationSettings> authenticationSettings)
        {
            _authenticationRepository = authenticationRepository;
            _appSettings = appSettings.Value;
            _authenticationSettings = authenticationSettings.Value;
        }

        public UserDto? AuthenticateUser(AuthenticateRequest req)
        {
            // Validate the authentication request
            new ValidatorExtensions.GenericValidation<AuthenticateRequest, AuthenticationValidator>().Validate(req);

            // Find the user by email (case-insensitive)
            var user = _authenticationRepository
                .Get(u => u.IsActive == true && u.Email.ToLower().Trim() == req.Username.ToLower().Trim(), includes: x => x.Role)
                .FirstOrDefault();

            // Check if user exists and password is valid
            if (user == null || !PasswordHash.ValidatePassword(req.Password, user.Password ?? ""))
                return null;

            // Generate JWT token and update user information
            var token = GenerateJwtToken(user);
            user.AccessToken = new JwtSecurityTokenHandler().WriteToken(token);
            user.RefreshToken = Guid.NewGuid().ToString();
            user.LastLoginTime = DateTime.Now;
            _authenticationRepository.InsertOrUpdateAsync(user.UserId, user);

            // Return user data as DTO
            return user.Adapt<UserDto>();
        }

        public async Task<ResponseMessage<UserDto>> RegisterUser(string email)
        {
            var response = new ResponseMessage<UserDto>();
            email = email.ToLower().Trim();

            // Check if user already exists
            var user = _authenticationRepository
                .Get(x => x.Email.ToLower().Trim() == email, includes: x => x.Role)
                .FirstOrDefault();

            if (user != null)
                return HandleExistingUser(user, response);

            // Create new user if not exists
            return await CreateNewUser(email, response);
        }

        public async Task<ResponseMessage<object>> UpdateUserLogin(UserAdd request)
        {
            // Validate the user add request
            new ValidatorExtensions.GenericValidation<UserAdd, UserAddValidator>().Validate(request);
            var response = new ResponseMessage<object>();

            // Find user by registration code
            var login = _authenticationRepository
                .Get(l => l.RegistrationCode == request.RegistrationCode)
                .FirstOrDefault();

            if (login == null)
            {
                response.SetErrorMessage("Invalid registration code.", "Validation Error");
                return response;
            }

            // Update user login details
            login.DisplayName = request.DisplayName;
            login.Password = PasswordHash.CreateHash(request.Password);
            login.IsActive = true;
            login.RegistrationCode = null;
            login.RegistrationCodeTime = null;

            await _authenticationRepository.InsertOrUpdateAsync(login.UserId, login);

            response.Success = true;
            response.Message = "User login details updated successfully.";
            return response;
        }

        public async Task<ResponseMessage<string>> ForgotPassword(string email)
        {
            var response = new ResponseMessage<string>();

            // Find user by email
            var user = _authenticationRepository.Get(x => x.Email == email).FirstOrDefault();
            if (user == null)
            {
                response.SetErrorMessage("Invalid User", "Not Found");
                return response;
            }

            // Generate new registration code
            var token = Guid.NewGuid().ToString("N");
            //TODO: send mail with token  

            // Update user with new registration code
            user.RegistrationCode = token;
            user.RegistrationCodeTime = DateTime.Now;
            await _authenticationRepository.InsertOrUpdateAsync(user.UserId, user);

            response.Success = true;
            response.Message = "Reset mail has been sent to the corresponding email.";
            return response;
        }

        public async Task<ResponseMessage<bool>> UpdatePassword(string token, string newPassword)
        {
            // Find user by registration code
            var user = _authenticationRepository.Get(x => x.RegistrationCode == token).FirstOrDefault();

            if (user == null)
                return new ResponseMessage<bool> { Data = false, Message = "Invalid Registration Code" };

            if (IsRegistrationCodeExpired(user))
                return new ResponseMessage<bool> { Data = false, Message = "Registration Code Expired" };

            // Update user password and reset registration code
            user.Password = PasswordHash.CreateHash(newPassword);
            user.RegistrationCode = null;
            user.RegistrationCodeTime = null;
            user.IsActive = true;
            await _authenticationRepository.InsertOrUpdateAsync(user.UserId, user);

            return new ResponseMessage<bool> { Data = true, Message = "Password has been Updated" };
        }

        private JwtSecurityToken GenerateJwtToken(User user)
        {
            // Create claims for the token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("userId", user.UserId.ToString()),
                new Claim("role", user.Role?.Name ?? ""),
                new Claim("expiryTime", DateTime.Now.AddMinutes(_appSettings.TokenValidity).ToString())
            };

            // Create and return the JWT token
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtBearer.SecurityKey));
            return new JwtSecurityToken(
                issuer: _authenticationSettings.JwtBearer.Issuer,
                audience: _authenticationSettings.JwtBearer.Audience,
                expires: DateTime.Now.AddMinutes(_appSettings.TokenValidity),
                claims: claims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
        }

        private ResponseMessage<UserDto> HandleExistingUser(User user, ResponseMessage<UserDto> response)
        {
            if (string.IsNullOrEmpty(user.Password))
            {
                if (IsRegistrationCodeExpired(user))
                {
                    // Generate new registration code if expired
                    user.RegistrationCode = Guid.NewGuid().ToString("N");
                    user.RegistrationCodeTime = DateTime.Now;
                    _authenticationRepository.InsertOrUpdateAsync(user.UserId, user);
                    //TODO: Send mail with new registration code
                    response.Message = "User already exists but the registration code was expired. A new code has been sent.";
                }
                else
                {
                    //TODO: Send mail with existing registration code
                    response.Message = "User already exists but has no password. An email with the registration code has been sent.";
                }
                response.Data = user.Adapt<UserDto>();
            }
            else
            {
                response.SetErrorMessage("Username/Email already exists.", "Validation Error");
            }
            return response;
        }

        private async Task<ResponseMessage<UserDto>> CreateNewUser(string email, ResponseMessage<UserDto> response)
        {
            // Get user role
            var roleId = _authenticationRepository.GetRole(UserRole.User.ToString()).RoleId;

            // Create new user
            var newUser = new User
            {
                Email = email,
                RoleId = roleId,
                RegistrationCode = Guid.NewGuid().ToString("N"),
                RegistrationCodeTime = DateTime.Now
            };

            var createdUser = await _authenticationRepository.InsertAsync(newUser);
            //TODO: Send welcome mail with registration code
            response.Data = createdUser.Adapt<UserDto>();
            response.Message = "Login created successfully";
            return response;
        }

        private bool IsRegistrationCodeExpired(User user)
        {
            // Check if registration code is expired based on app settings
            return user.RegistrationCode != null &&
                   user.RegistrationCodeTime.HasValue &&
                   DateTime.Now - user.RegistrationCodeTime.Value > TimeSpan.FromMinutes(_appSettings.RegistrationCodeValidity);
        }
    }
}