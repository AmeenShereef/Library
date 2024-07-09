using BookLibrary.Data.Entities;

namespace BookLibrary.Repositories.Abstractions
{
    public interface IAuthenticationRepository : IBaseRepository<User>
    {
        public Role GetRole(string role);
    }
}
