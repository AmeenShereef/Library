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
        ResponseMessage<List<BookCopyDto>> GetAllBookCopy(int bookId);
        ResponseMessage<PagedList<BookDto>> GetBooks(int? pageNumber, int? pageSize, string orderBy, bool orderDirection, string search);
        Task<ResponseMessage<BorrowedBookDto>> BorrowBook(BorrowBookReq req);
        Task<ResponseMessage<List<BorrowedBookDto>>> GetUserBorrowedBook(int userId,bool isReturned);
        Task<ResponseMessage<BorrowedBookDto>> ReturnBook(int borrowedBookId);

        //admin 

        Task<ResponseMessage<Book>> AddOrUpdateBook(BookAdd book);
        Task<ResponseMessage<BookCopy>> AddOrUpdateBookCopy(BookCopyAdd bc);
        Task<ResponseMessage<bool>> DeleteBookCopy(int id);
        Task<ResponseMessage<bool>> DeleteBook(int id);
    }
}
