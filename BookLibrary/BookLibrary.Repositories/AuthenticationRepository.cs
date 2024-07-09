using BookLibrary.Data.Entities;
using BookLibrary.Repositories.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BookLibrary.Repositories
{
    public class AuthenticationRepository : BaseRepository<User>, IAuthenticationRepository
    {
        public AuthenticationRepository(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, ILogger<AuthenticationRepository> logger) : base(unitOfWork, httpContextAccessor, logger)
        {
        }
    }
}
