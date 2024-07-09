using BookLibrary.Business.Constants;
using BookLibrary.Data.Entities;
using BookLibrary.Models;

namespace BookLibrary.Business.Abstractions
{
    public interface IAuthenticationService
    {
        public UserDto? AuthenticateUser(AuthenticateRequest authenicateRequest);
        public Task<ResponseMessage<object>> UpdateUserLogin(UserAdd request);
        public Task<ResponseMessage<UserDto>> RegisterUser(string email);
        public Task<ResponseMessage<string>> ForgotPassword(string email);
        public Task<ResponseMessage<bool>> UpdatePassword(string token, string newPassword);

    }
}
