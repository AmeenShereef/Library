using BookLibrary.Business.Abstractions;
using BookLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookLibrary.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IBookService _bookService;

        /// <summary>
        /// Initializes a new instance of the bookController.
        /// </summary>
        /// <param name="bookService">The bookService service.</param>
        /// <param name="logger">The logger.</param>
        public BookController(IBookService bookService, ILogger<AuthenticationController> logger)
        {
            _bookService = bookService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet("GetBooks")]
        public IActionResult GetBooks(int? pageNumber, int? pageSize, string orderBy ="BookId", bool orderDirection = true, string search = "")
        {
            _logger.LogInformation("Entering GetBooks");
            var books = _bookService.GetBooks(pageNumber, pageSize, orderBy, orderDirection, search);
            _logger.LogInformation("Retrieved All Books");

            return Ok(books);
        }

        [AllowAnonymous]
        [HttpPost("BorrowBook")]
        public async Task<ActionResult> BorrowBook(BorrowBookReq req)
        {
            _logger.LogInformation("Entering BorrowBook");
            var books = await _bookService.BorrowBook(req);
            _logger.LogInformation("leaving BorrowBook");

            return Ok(books);
        }
        [AllowAnonymous]
        [HttpGet("GetUserBorrowedBook")]
        public IActionResult GetUserBorrowedBook(int userId,bool isReturned = false)
        {
            _logger.LogInformation("Entering GetUserBorrowedBook");
            var books = _bookService.GetUserBorrowedBook(userId,isReturned);
            _logger.LogInformation("leaving GetUserBorrowedBook");

            return Ok(books);
        }
    }
}
