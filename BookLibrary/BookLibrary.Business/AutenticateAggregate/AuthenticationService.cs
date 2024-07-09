using BookLibrary.Business.Abstractions;
using BookLibrary.Business.Constants;
using BookLibrary.Business.Extensions;
using BookLibrary.Business.Services.PasswordHasher;
using BookLibrary.Data.Entities;
using BookLibrary.Infrastructure.AppSettings;
using BookLibrary.Models;
using BookLibrary.Repositories;
using BookLibrary.Repositories.Abstractions;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookLibrary.Business.AutenticateAggregate
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly AppSettings _appSettings;
        private readonly AuthenticationSettings _authenticationSettings ;


        public AuthenticationService(IAuthenticationRepository authenticationRepository,IConfiguration configuration,IOptions<AppSettings> appSettings, IOptions<AuthenticationSettings> authenticationSettings)
        {
            this._authenticationRepository = authenticationRepository;
            this._appSettings = appSettings.Value;
            this._authenticationSettings = authenticationSettings.Value;
        }

        public UserDto? AuthenticateUser(AuthenticateRequest req)
        {
            new ValidatorExtensions.GenericValidation<AuthenticateRequest, AuthenticationValidator>().Validate(req);

            var user = _authenticationRepository
                .Get(u => u.IsActive == true && u.Email.ToLower().Trim() == req.Username.ToLower().Trim(),includes: x => x.Role)
                .FirstOrDefault();
            var password = req.Password;

            if (user == null || (!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(user.Password) && !PasswordHash.ValidatePassword(password, user.Password)))
                return null;
            
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),                
                new Claim("userId", user.UserId.ToString()),
                new Claim("role", user.Role?.Name ??""),
                new Claim("expiryTime", DateTime.Now.AddMinutes(_appSettings.TokenValidity).ToString())
            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtBearer.SecurityKey));
            var token = new JwtSecurityToken(
                issuer: _authenticationSettings.JwtBearer.Issuer,
                audience: _authenticationSettings.JwtBearer.Audience,
                expires: DateTime.Now.AddMinutes(_appSettings.TokenValidity),
                claims: claims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            user.AccessToken = new JwtSecurityTokenHandler().WriteToken(token);
            user.RefreshToken = Guid.NewGuid().ToString();
            user.LastLoginTime = DateTime.Now;
            _authenticationRepository.InsertOrUpdateAsync(user.UserId, user);
            return user.Adapt<UserDto>();
            
        }
        public async Task<ResponseMessage<UserDto>> RegisterUser(string email)
        {           
            var response = new ResponseMessage<UserDto>();
            var newToken = Guid.NewGuid().ToString("N");
            var currentTime = DateTime.Now;

            // Normalize email
            email = email.ToLower().Trim();

            // Check if a user with the given email already exists 
            var user = _authenticationRepository
                .Get(x => x.Email.ToLower().Trim() == email, includes: x => x.Role)
                .FirstOrDefault();

            if (user != null)
            {
                // User exists but no password is set
                if (string.IsNullOrEmpty(user.Password))
                {
                    // If registration code has expired
                    if (user.RegistrationCode != null &&
                        user.RegistrationCodeTime.HasValue &&
                        currentTime - user.RegistrationCodeTime.Value > TimeSpan.FromMinutes(_appSettings.RegistrationCodeValidity))
                    {
                        user.RegistrationCode = newToken;
                        user.RegistrationCodeTime = currentTime;
                        await _authenticationRepository.InsertOrUpdateAsync(user.UserId, user);
                        
                        // TODO: Send mail with new registration code                       

                        response.Data = user.Adapt<UserDto>();
                        response.Message = "User already exists but the registration code was expired. A new code has been sent.";
                        return response;
                    }
                    else
                    {
                        // TODO: Send mail with existing registration code
                        
                        response.Data = user.Adapt<UserDto>();
                        response.Message = "User already exists but has no password. An email with the registration code has been sent.";
                        return response;
                    }
                }
                else
                {
                    // If user is present with both username and password
                    response.SetErrorMessage("Username/Email already exists.", "Validation Error");
                    return response;
                }
            }

            var roleId = _authenticationRepository.GetRole(UserRole.User.ToString().ToString()).RoleId;

            user = new User
            {
                Email = email,
                RoleId = roleId,
                RegistrationCode = newToken,
                RegistrationCodeTime = currentTime
            };

            var createdUser = await _authenticationRepository.InsertAsync(user);

            // TODO: Send welcome mail with registration code
            response.Data = createdUser.Adapt<UserDto>();
            response.Message = "Login created successfully";
            return response;
        }


        public async Task<ResponseMessage<object>> UpdateUserLogin(UserAdd request)
        {
            var response = new ResponseMessage<object>();

            var login = _authenticationRepository
                .Get(l => l.RegistrationCode == request.RegistrationCode)
                .FirstOrDefault();

            if (login == null)
            {
                response.SetErrorMessage("Invalid registration code.", "Validation Error");
                return response;
            }

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



    }
}
