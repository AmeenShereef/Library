using BookLibrary.Business.Abstractions;
using BookLibrary.Business.Extensions;
using BookLibrary.Data;
using BookLibrary.Data.Entities;
using BookLibrary.Models;
using BookLibrary.Repositories;
using BookLibrary.Repositories.Abstractions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BookLibrary.Business.BookAggregate
{
    public class BookService:IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IBorrowBookRepository _borrowBookRepository;
        private readonly IBookCopyRepository _bookCopyRepository;
        public BookService(IBookRepository bookRepository,IBorrowBookRepository borrowBookRepository, IBookCopyRepository bookCopyRepository)
        {
            _bookRepository = bookRepository;
            _borrowBookRepository = borrowBookRepository;
            _bookCopyRepository = bookCopyRepository;
        }

        

        public async Task<ResponseMessage<PagedList<BookDto>>> GetBooks(int? pageNumber, int? pageSize, string orderBy , bool orderDirection, string search )
        {
            search = search.ToLower();

            // Building the search predicate
            Expression<Func<Book, bool>> searchPredicate = x =>
                (string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search)) ||
                (!string.IsNullOrEmpty(x.Genre) && x.Genre.ToLower().Contains(search)) ||
                (!string.IsNullOrEmpty(x.ISBN) && x.ISBN.ToLower().Contains(search)) ||
                (x.PublicationYear != null && x.PublicationYear!.ToString()!.ToLower().Contains(search));

            // Include BookCopies
            Expression<Func<Book, object>> includeExpression = b => b.BookCopies;

            // Fetching the books from repository
            var books = _bookRepository.GetAll(pageNumber,pageSize,orderBy,orderDirection,searchPredicate,b => b.BookCopies.Where(bc => bc.IsAvailable).Take(1));

            return new ResponseMessage<PagedList<BookDto>>() { Data = books.Adapt<PagedList<BookDto>>() }; 
        }

        public async Task<ResponseMessage<BorrowedBook>> BorrowBook(BorrowBookReq req)
        {
            if (req == null)
                return new ResponseMessage<BorrowedBook> { Success = false, Message = "Invalid request object." };

            // Validate the request
            new ValidatorExtensions.GenericValidation<BorrowBookReq, BorrowBookValidator>().Validate(req);

            // Get the user ID
            var userId = _borrowBookRepository.GetUserId();

            // Get and update the book copy
            var bookCopy = await _bookCopyRepository.GetByIdAsync(req.BookCopyId);
            if (bookCopy == null)
                return new ResponseMessage<BorrowedBook> { Success = false, Message = "Book copy not found." };

            if (!bookCopy.IsAvailable)
                return new ResponseMessage<BorrowedBook> { Success = false, Message = "Book copy is not available." };

            bookCopy.IsAvailable = false;

            // Create and insert the borrowed book record
            var borrowedBook = req.Adapt<BorrowedBook>();
            var currentDate = DateTime.Now;
            borrowedBook.UserId = (int)userId;
            borrowedBook.BorrowDate = currentDate;
            borrowedBook.DueDate = currentDate.AddMonths(1); // Assuming the return due will be 1 month

            // Perform both updates in a single operation
            await _bookCopyRepository.InsertOrUpdateAsync(bookCopy.BookCopyId, bookCopy);
            await _borrowBookRepository.InsertOrUpdateAsync(borrowedBook.BorrowedBookId, borrowedBook);


            return new ResponseMessage<BorrowedBook> { Success = true, Data = borrowedBook };
        }


        public ResponseMessage<List<BorrowedBookDto>> GetUserBorrowedBook(int userId, bool isReturned)
        {
           var borBook = _borrowBookRepository.GetAll()
                .Where(x => x.UserId == userId && isReturned ? x.ReturnDate!=null: x.ReturnDate == null )
                .Include(x => x.BookCopy)
                .Include( x => x.BookCopy.Book)
                .ToList();

           return new ResponseMessage<List<BorrowedBookDto>>() { Data = borBook.Adapt<List<BorrowedBookDto>>() };
        }


    }
}
