using BookLibrary.Data.Entities;
using BookLibrary.Repositories.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibrary.Repositories
{
    public class BookRepository : BaseRepository<Book>, IBookRepository
    {
        public BookRepository(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, ILogger<BaseRepository<Book>> logger) : base(unitOfWork, httpContextAccessor, logger)
        {
        }
        
    }
}
