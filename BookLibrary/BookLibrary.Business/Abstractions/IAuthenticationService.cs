using BookLibrary.Models;

namespace BookLibrary.Business.Abstractions
{
    public interface IAuthenticationService
    {
        public UserDto? GetAuthenticatedUser(AuthenticateRequest authenicateRequest);
        public Task<ResponseMessage<object>> UpdateUserLogin(UserAdd request);
        public Task<ResponseMessage<UserDto>> CreateLogin(string email);
        
    }
}
