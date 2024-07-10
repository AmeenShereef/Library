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
    public class BorrowBookRepository : BaseRepository<BorrowedBook>, IBorrowBookRepository
    {
        public BorrowBookRepository(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, ILogger<BaseRepository<BorrowedBook>> logger) : base(unitOfWork, httpContextAccessor, logger)
        {
        }
    }
}
