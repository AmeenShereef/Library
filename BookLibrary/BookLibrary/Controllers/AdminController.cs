using BookLibrary.Business.Abstractions;
using BookLibrary.Data.Entities;
using BookLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookLibrary.API.Controllers
{
    /// <summary>
    /// Controller for handling admin-related operations in the Book Library API.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IBookService _bookService;
        private readonly IAuthenticationService _authenticationService;

        /// <summary>
        /// Initializes a new instance of the AdminController.
        /// </summary>
        /// <param name="bookService">The book service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="authenticationService">The authentication service.</param>
        public AdminController(IBookService bookService, ILogger<AuthenticationController> logger, IAuthenticationService authenticationService)
        {
            _bookService = bookService;
            _logger = logger;
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Adds or updates a book in the library.
        /// </summary>
        /// <param name="req">The book details to add or update.</param>
        /// <returns>A response message containing the added or updated book.</returns>    
        [HttpPost("AddOrUpdateBook")]
        public async Task<ResponseMessage<Book>> AddOrUpdateBook(BookAdd req)
        {
            _logger.LogInformation("Entering AddOrUpdateBook");
            var books = await _bookService.AddOrUpdateBook(req);
            _logger.LogInformation("Leaving AddOrUpdateBook");
            return books;
        }

        /// <summary>
        /// Retrieves all copies of a specific book.
        /// </summary>
        /// <param name="bookId">The ID of the book.</param>
        /// <returns>A response message containing a list of book copies.</returns>
        [HttpPost("GetAllBookCopy/{bookId}")]
        public Task<ResponseMessage<List<BookCopyDto>>> GetAllBookCopy(int bookId)
        {
            _logger.LogInformation("Entering GetAllBookCopy");
            var books = _bookService.GetAllBookCopy(bookId);
            _logger.LogInformation("Leaving GetAllBookCopy");
            return books;
        }

        /// <summary>
        /// Adds or updates a copy of a book in the library.
        /// </summary>
        /// <param name="req">The book copy details to add or update.</param>
        /// <returns>A response message containing the added or updated book copy.</returns>
        [HttpPost("AddOrUpdateBookCopy")]
        public async Task<ResponseMessage<BookCopy>> AddOrUpdateBookCopy(BookCopyAdd req)
        {
            _logger.LogInformation("Entering AddOrUpdateBookCopy");
            var books = await _bookService.AddOrUpdateBookCopy(req);
            _logger.LogInformation("Leaving AddOrUpdateBookCopy");
            return books;
        }

        /// <summary>
        /// Deletes a book from the library.
        /// </summary>
        /// <param name="bookId">The ID of the book to delete.</param>
        /// <returns>A response message indicating whether the deletion was successful.</returns>
        [HttpDelete("DeleteBook/{bookId}")]
        public async Task<ResponseMessage<bool>> DeleteBook(int bookId)
        {
            _logger.LogInformation("Entering DeleteBook");
            var isDeleted = await _bookService.DeleteBook(bookId);
            _logger.LogInformation("Leaving DeleteBook");
            return isDeleted;
        }

        /// <summary>
        /// Deletes a copy of a book from the library.
        /// </summary>
        /// <param name="bookCopyId">The ID of the book copy to delete.</param>
        /// <returns>A response message indicating whether the deletion was successful.</returns>
        [HttpDelete("DeleteBookCopy/{bookCopyId}")]
        public async Task<ResponseMessage<bool>> DeleteBookCopy(int bookCopyId)
        {
            _logger.LogInformation("Entering DeleteBookCopy");
            var isDeleted = await _bookService.DeleteBook(bookCopyId);
            _logger.LogInformation("Leaving DeleteBookCopy");
            return isDeleted;
        }

        /// <summary>
        /// Registers a new user with the specified email.
        /// </summary>
        /// <param name="email">The email of the user to be registered.</param>
        /// <returns>A response message containing the registered user details.</returns>
        [HttpPost("RegisterUser/{email}")]
        public async Task<ResponseMessage<UserDto>> RegisterUser(string email)
        {
            _logger.LogInformation("Entering RegisterUser");
            var result = await _authenticationService.RegisterUser(email);
            _logger.LogInformation("Leaving RegisterUser");
            return result;
        }
    }
}