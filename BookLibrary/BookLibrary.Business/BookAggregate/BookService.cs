using BookLibrary.Business.Abstractions;
using BookLibrary.Business.Extensions;
using BookLibrary.Data;
using BookLibrary.Data.Entities;
using BookLibrary.Models;
using BookLibrary.Repositories.Abstractions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Net;

namespace BookLibrary.Business.BookAggregate
{
    /// <summary>
    /// Service class for handling book-related operations.
    /// </summary>
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IBorrowBookRepository _borrowBookRepository;
        private readonly IBookCopyRepository _bookCopyRepository;

        public BookService(IBookRepository bookRepository, IBorrowBookRepository borrowBookRepository, IBookCopyRepository bookCopyRepository)
        {
            _bookRepository = bookRepository;
            _borrowBookRepository = borrowBookRepository;
            _bookCopyRepository = bookCopyRepository;
        }

        public ResponseMessage<PagedList<BookDto>> GetBooks(int? pageNumber, int? pageSize, string orderBy, bool orderDirection, string search)
        {
            // Convert search term to lowercase for case-insensitive search
            search = search.ToLower();

            // Define search predicate
            Expression<Func<Book, bool>> searchPredicate = x => x.IsActive && (
                string.IsNullOrEmpty(search) ||
                x.Title.ToLower().Contains(search) ||
                (!string.IsNullOrEmpty(x.Genre) && x.Genre.ToLower().Contains(search)) ||
                (!string.IsNullOrEmpty(x.ISBN) && x.ISBN.ToLower().Contains(search)) ||
                (x.PublicationYear != null && x.PublicationYear.ToString()!.ToLower().Contains(search)));

            // Retrieve books from repository
            var books = _bookRepository.GetAll(pageNumber, pageSize, orderBy, orderDirection, searchPredicate, b => b.BookCopies.Where(bc => bc.IsAvailable && bc.IsActive).Take(1));

            // Map and return results
            return new ResponseMessage<PagedList<BookDto>>
            {
                Data = books.Adapt<PagedList<BookDto>>(),
                Success = true,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Books retrieved successfully"
            };
        }

        public  ResponseMessage<List<BookCopyDto>> GetAllBookCopy(int bookId)
        {
            // Retrieve all copies of the specified book
            var books = _bookCopyRepository.Get(x => x.BookId == bookId).ToList();

            // Map and return results
            return new ResponseMessage<List<BookCopyDto>>
            {
                Data = books.Adapt<List<BookCopyDto>>(),
                Success = true,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Book copies retrieved successfully"
            };
        }

        public async Task<ResponseMessage<Book>> AddOrUpdateBook(BookAdd book)
        {
            // Validate the book data
            new ValidatorExtensions.GenericValidation<BookAdd, BookValidator>().Validate(book);

            // Map to entity and set as active
            var entity = book.Adapt<Book>();
            entity.IsActive = true;

            // Insert or update the book
            var res = await _bookRepository.InsertOrUpdateAsync(entity.BookId, entity);

            // Return response
            return new ResponseMessage<Book>
            {
                Success = true,
                Data = res,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Book saved successfully"
            };
        }

        public async Task<ResponseMessage<BookCopy>> AddOrUpdateBookCopy(BookCopyAdd bc)
        {
            // Validate the book copy data
            new ValidatorExtensions.GenericValidation<BookCopyAdd, BookCopyValidator>().Validate(bc);

            // Map to entity and set as active
            var entity = bc.Adapt<BookCopy>();
            entity.IsActive = true;

            // Insert or update the book copy
            var res = await _bookCopyRepository.InsertOrUpdateAsync(entity.BookCopyId, entity);

            // Return response
            return new ResponseMessage<BookCopy>
            {
                Success = true,
                Data = res,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Book copy saved successfully"
            };
        }

        public async Task<ResponseMessage<bool>> DeleteBook(int id)
        {
            // Retrieve the book
            var book = await _bookRepository.GetByIdAsync(id);

            // Check if book exists
            if (book == null)
                return new ResponseMessage<bool>
                {
                    Data = false,
                    Success = false,
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "Invalid book Id. Book not found."
                };

            // Check if book is already deleted
            if (!book.IsActive)
                return new ResponseMessage<bool>
                {
                    Data = false,
                    Success = false,
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "Book is already deleted."
                };

            // Soft delete the book
            await _bookRepository.SoftDeleteAsync(book);

            // Return success response
            return new ResponseMessage<bool>
            {
                Data = true,
                Success = true,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Book deleted successfully"
            };
        }

        public async Task<ResponseMessage<bool>> DeleteBookCopy(int id)
        {
            // Retrieve the book copy
            var bookCopy = await _bookCopyRepository.GetByIdAsync(id);

            // Check if book copy exists
            if (bookCopy == null)
                return new ResponseMessage<bool>
                {
                    Data = false,
                    Success = false,
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "Invalid book copy Id. Book copy not found."
                };

            // Check if book copy is already deleted
            if (!bookCopy.IsActive)
                return new ResponseMessage<bool>
                {
                    Data = false,
                    Success = false,
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "Book copy is already deleted."
                };

            // Soft delete the book copy
            await _bookCopyRepository.SoftDeleteAsync(bookCopy);

            // Return success response
            return new ResponseMessage<bool>
            {
                Data = true,
                Success = true,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Book copy deleted successfully"
            };
        }

        public async Task<ResponseMessage<BorrowedBookDto>> BorrowBook(BorrowBookReq req)
        {
            // Validate request
            if (req == null)
                return new ResponseMessage<BorrowedBookDto>
                {
                    Success = false,
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "Invalid request object."
                };

            // Validate borrow request data
            new ValidatorExtensions.GenericValidation<BorrowBookReq, BorrowBookValidator>().Validate(req);

            // Get user ID and book copy
            var userId = _borrowBookRepository.GetUserId();
            var bookCopy = await _bookCopyRepository.GetByIdAsync(req.BookCopyId);

            // Check if book copy exists
            if (bookCopy == null)
                return new ResponseMessage<BorrowedBookDto>
                {
                    Success = false,
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "Book copy not found."
                };

            // Check if book copy is available
            if (!bookCopy.IsAvailable)
                return new ResponseMessage<BorrowedBookDto>
                {
                    Success = false,
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "Book copy is not available."
                };

            // Check if user has already borrowed a copy of this book
            bool alreadyBorrowed =  _borrowBookRepository.Get(x => x.UserId == userId && x.ReturnDate == null && x.BookCopy.Book.BookId == bookCopy.BookId).Any();
            if (alreadyBorrowed)
                return new ResponseMessage<BorrowedBookDto>
                {
                    Success = false,
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "Another copy of the same book has been borrowed and not returned."
                };

            // Update book copy availability
            bookCopy.IsAvailable = false;

            // Create borrowed book record
            var borrowedBook = req.Adapt<BorrowedBook>();
            var currentDate = DateTime.Now;
            borrowedBook.UserId = (int)userId;
            borrowedBook.BorrowDate = currentDate;
            borrowedBook.DueDate = currentDate.AddMonths(1);

            // Save changes
            await Task.WhenAll(
                _bookCopyRepository.InsertOrUpdateAsync(bookCopy.BookCopyId, bookCopy),
                _borrowBookRepository.InsertOrUpdateAsync(borrowedBook.BorrowedBookId, borrowedBook)
            );

            // Return success response
            return new ResponseMessage<BorrowedBookDto>
            {
                Success = true,
                Data = borrowedBook.Adapt<BorrowedBookDto>(),
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Book borrowed successfully"
            };
        }

        public async Task<ResponseMessage<List<BorrowedBookDto>>> GetUserBorrowedBook(int userId, bool isReturned)
        {
            // Retrieve borrowed books for the user
            var borBook = await _borrowBookRepository.GetAll()
                .Where(x => x.UserId == userId && (isReturned ? x.ReturnDate != null : x.ReturnDate == null))
                .Include(x => x.BookCopy)
                    .ThenInclude(bc => bc.Book)
                .AsNoTracking()
                .ToListAsync();

            // Map to DTOs
            var borrowedBookDtos = borBook.Adapt<List<BorrowedBookDto>>();

            // Return response
            return new ResponseMessage<List<BorrowedBookDto>>
            {
                Data = borrowedBookDtos,
                Success = true,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "User borrowed books retrieved successfully"
            };
        }

        public async Task<ResponseMessage<BorrowedBookDto>> ReturnBook(int borrowedBookId)
        {
            // Retrieve borrowed book record
            var borBook = _borrowBookRepository.Get(x => x.BorrowedBookId == borrowedBookId).FirstOrDefault();
            if (borBook == null)
            {
                return new ResponseMessage<BorrowedBookDto>
                {
                    Success = false,
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "Invalid borrowedBookId"
                };
            }

            // Set return date
            borBook.ReturnDate = DateTime.Now;

            // Retrieve and update book copy
            var bookCopy = await _bookCopyRepository.GetByIdAsync(borBook.BookCopyId);
            if (bookCopy == null)
            {
                return new ResponseMessage<BorrowedBookDto>
                {
                    Success = false,
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "Book copy not found"
                };
            }
            bookCopy.IsAvailable = true;

            // Save changes
            await Task.WhenAll(
                _borrowBookRepository.InsertOrUpdateAsync(borrowedBookId, borBook),
                _bookCopyRepository.InsertOrUpdateAsync(bookCopy.BookCopyId, bookCopy)
            );

            // Return success response
            return new ResponseMessage<BorrowedBookDto>
            {
                Success = true,
                Data = borBook.Adapt<BorrowedBookDto>(),
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Book returned successfully"
            };
        }
    }
}