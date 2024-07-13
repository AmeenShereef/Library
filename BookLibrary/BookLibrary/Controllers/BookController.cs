using BookLibrary.Business.Abstractions;
using BookLibrary.Data;
using BookLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BookLibrary.API.Controllers
{
    /// <summary>
    /// Controller for handling book-related operations in the Book Library API.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IBookService _bookService;

        /// <summary>
        /// Initializes a new instance of the BookController.
        /// </summary>
        /// <param name="bookService">The book service.</param>
        /// <param name="logger">The logger.</param>
        public BookController(IBookService bookService, ILogger<AuthenticationController> logger)
        {
            _bookService = bookService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a paged list of books.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="orderBy">The field to order the results by. Defaults to "BookId".</param>
        /// <param name="orderDirection">The direction of ordering. True for ascending, false for descending.</param>
        /// <param name="search">The search string to filter books.</param>
        /// <returns>A response message containing a paged list of books.</returns>       
        [HttpGet("GetBooks")]
        [AllowAnonymous]
        public ActionResult GetBooks(int? pageNumber, int? pageSize, string orderBy = "BookId", bool orderDirection = true, string search = "")
        {
            try
            {
                _logger.LogInformation("Entering GetBooks");
                var books = _bookService.GetBooks(pageNumber, pageSize, orderBy, orderDirection, search);
                _logger.LogInformation("Retrieved All Books");
                return Ok(books);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
           
        }

        /// <summary>
        /// Borrows a book for a user.
        /// </summary>
        /// <param name="req">The borrow book request details.</param>
        /// <returns>A response message containing the borrowed book details.</returns>
        [HttpPost("BorrowBook")]
        public async Task<ResponseMessage<BorrowedBookDto>> BorrowBook(BorrowBookReq req)
        {
            _logger.LogInformation("Entering BorrowBook");
            var books = await _bookService.BorrowBook(req);
            _logger.LogInformation("Leaving BorrowBook");
            return books;
        }

        /// <summary>
        /// Retrieves the list of books borrowed by a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="isReturned">Flag to indicate whether to retrieve returned or currently borrowed books.</param>
        /// <returns>A response message containing a list of borrowed books.</returns>
        [HttpGet("GetUserBorrowedBook")]
        public Task<ResponseMessage<List<BorrowedBookDto>>> GetUserBorrowedBook(bool isReturned = false)
        {
            _logger.LogInformation("Entering GetUserBorrowedBook");
            var book = _bookService.GetUserBorrowedBook(isReturned);
            _logger.LogInformation("Leaving GetUserBorrowedBook");
            return book;
        }

        /// <summary>
        /// Returns a borrowed book.
        /// </summary>
        /// <param name="borrowedBookId">The ID of the borrowed book to return.</param>
        /// <returns>A response message containing the returned book details.</returns>
        [HttpPut("ReturnBook/{borrowedBookId}")]
        public async Task<ResponseMessage<BorrowedBookDto>> ReturnBook(int borrowedBookId)
        {
            _logger.LogInformation("Entering ReturnBook");
            var response = await _bookService.ReturnBook(borrowedBookId);
            _logger.LogInformation("Leaving ReturnBook");
            return response;
        }
    }
}