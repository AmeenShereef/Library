using BookLibrary.Data;
using BookLibrary.Data.Entities;
using BookLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibrary.Business.Abstractions
{
    public interface IBookService
    {
        Task<ResponseMessage<PagedList<BookDto>>> GetBooks(int? pageNumber, int? pageSize, string orderBy, bool orderDirection, string search);
        Task<ResponseMessage<BorrowedBook>> BorrowBook(BorrowBookReq req);
        ResponseMessage<List<BorrowedBookDto>> GetUserBorrowedBook(int userId,bool isReturned);
    }
}
