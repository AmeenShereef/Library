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
    public class BookCopyRepository : BaseRepository<BookCopy>, IBookCopyRepository
    {
        public BookCopyRepository(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, ILogger<BaseRepository<BookCopy>> logger) : base(unitOfWork, httpContextAccessor, logger)
        {
        }
    }
}
