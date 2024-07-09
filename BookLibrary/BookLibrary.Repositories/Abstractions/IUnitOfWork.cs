using BookLibrary.Data;

namespace BookLibrary.Repositories.Abstractions
{
    public interface IUnitOfWork
    {
        public APIContext _context { get; }
        Task CommitAsync();
        void Dispose();
    }
}