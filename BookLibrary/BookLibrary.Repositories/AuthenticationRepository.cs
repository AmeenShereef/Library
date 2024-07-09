using BookLibrary.Data.Entities;
using BookLibrary.Repositories.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Mail;

namespace BookLibrary.Repositories
{
    public class AuthenticationRepository : BaseRepository<User>, IAuthenticationRepository
    {
        public AuthenticationRepository(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, ILogger<AuthenticationRepository> logger) : base(unitOfWork, httpContextAccessor, logger)
        {

        }

        public  Role GetRole(string roleName)
        {
            Role? entity =  _unitOfWork._context.Set<Role>().Where(r => r.Name == roleName).SingleOrDefault();
            if (entity == null)
            {
                throw new KeyNotFoundException($"Role {roleName} not found");
            }
            return entity;
        }
    }
}
