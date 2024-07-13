using BookLibrary.Business.BookAggregate;
using BookLibrary.Data;
using BookLibrary.Data.Entities;
using BookLibrary.Models;
using BookLibrary.Repositories.Abstractions;
using FluentAssertions;
using Mapster;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace BookLibrary.Tests
{
    public class BookServiceTests
    {
        private readonly Mock<IBookRepository> _mockBookRepo;
        private readonly Mock<IBorrowBookRepository> _mockBorrowBookRepo;
        private readonly Mock<IBookCopyRepository> _mockBookCopyRepo;
        private readonly BookService _bookService;

        public BookServiceTests()
        {
            _mockBookRepo = new Mock<IBookRepository>();
            _mockBorrowBookRepo = new Mock<IBorrowBookRepository>();
            _mockBookCopyRepo = new Mock<IBookCopyRepository>();
            _bookService = new BookService(_mockBookRepo.Object, _mockBorrowBookRepo.Object, _mockBookCopyRepo.Object);
        }

        
        [Fact]
        public void GetBooks_WithValidSearch_ReturnsPagedListOfBookDto()
        {
            // Arrange
            var books = new List<Book>
            {
                new Book { BookId = 1, Title = "Test Book 1", IsActive = true ,Genre="test",PublicationYear =1999,ISBN= "dfs"},
                new Book { BookId = 2, Title = "Test Book 2", IsActive = true ,Genre="test",PublicationYear =1999,ISBN= "dfs"}
            }.AsQueryable();

            _mockBookRepo.Setup(r => r.GetAll(
             It.IsAny<int?>(),
             It.IsAny<int?>(),
             It.IsAny<string>(),
             It.IsAny<bool>(),
             It.IsAny<Expression<Func<Book, bool>>>()             
         ))
         .Returns((int? pageIndex, int? pageSize, string orderBy, bool orderAsc, Expression<Func<Book, bool>> filter, Func<IQueryable<Book>, IQueryable<Book>> include) => {
             // Your mock repository logic here
             IQueryable<Book> filteredBooks = books;

             // Apply filtering if necessary based on the 'filter' expression
             if (filter != null)
             {
                 filteredBooks = filteredBooks.Where(filter);
             }

             // Apply any include logic if needed
             if (include != null)
             {
                 filteredBooks = include(filteredBooks);
             }

             // Create and return a PagedList<Book> based on the filteredBooks
             return new PagedList<Book>(filteredBooks.ToList(), pageIndex ?? 1, pageSize ?? 2, filteredBooks.Count());
             });




            // Act
            var result = _bookService.GetBooks(1, 2, "Title", true, "Test");

            // Assert
            result.Should().NotBeNull();
            result.Data?.Items.Should().BeAssignableTo<IEnumerable<BookDto>>();  // Ensure Data is a collection
            result.Data?.Items.Should().HaveCount(2);
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }


        [Fact]
        public  void GetAllBookCopy_WithValidBookId_ReturnsListOfBookCopyDto()
        {
            // Arrange
            var bookCopies = new List<BookCopy>
            {
                new BookCopy { BookCopyId = 1, BookId = 1, IsActive = true, IsAvailable = true },
                new BookCopy { BookCopyId = 2, BookId = 1, IsActive = true, IsAvailable = false }
            }.AsQueryable();

            _mockBookCopyRepo.Setup(r => r.Get(It.IsAny<Expression<Func<BookCopy, bool>>>()))
                .Returns(bookCopies);

            // Act
            var result = _bookService.GetAllBookCopy(1);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().HaveCount(2);
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public async Task AddOrUpdateBook_WithValidBookAdd_ReturnsResponseMessageWithBook()
        {
            // Arrange
            var bookAdd = new BookAdd { Title = "New Book", Genre = "Fiction",Author="Raj", ISBN = "123" ,PublicationYear = 123};
            var book = bookAdd.Adapt<Book>();
            book.BookId = 1;
            book.IsActive = true;

            _mockBookRepo.Setup(r => r.InsertOrUpdateAsync(It.IsAny<int>(), It.IsAny<Book>()))
                .ReturnsAsync(book);

            // Act
            var result = await _bookService.AddOrUpdateBook(bookAdd);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().NotBeNull();
            result.Data?.BookId.Should().Be(1);
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public async Task AddOrUpdateBookCopy_WithValidBookCopyAdd_ReturnsResponseMessageWithBookCopy()
        {
            // Arrange
            var bookCopyAdd = new BookCopyAdd { BookId = 1, CopyNumber = 1 };
            var bookCopy = bookCopyAdd.Adapt<BookCopy>();
            bookCopy.BookCopyId = 1;
            bookCopy.IsActive = true;

            _mockBookCopyRepo.Setup(r => r.InsertOrUpdateAsync(It.IsAny<int>(), It.IsAny<BookCopy>()))
                .ReturnsAsync(bookCopy);

            // Act
            var result = await _bookService.AddOrUpdateBookCopy(bookCopyAdd);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().NotBeNull();
            result.Data?.BookCopyId.Should().Be(1);
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteBook_WithValidId_ReturnsSuccessResponse()
        {
            // Arrange
            var book = new Book { BookId = 1, IsActive = true };
            _mockBookRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(book);
            _mockBookRepo.Setup(r => r.SoftDeleteAsync(It.IsAny<Book>())).Returns(Task.CompletedTask);

            // Act
            var result = await _bookService.DeleteBook(1);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().BeTrue();
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteBookCopy_WithValidId_ReturnsSuccessResponse()
        {
            // Arrange
            var bookCopy = new BookCopy { BookCopyId = 1, IsActive = true };
            _mockBookCopyRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(bookCopy);
            _mockBookCopyRepo.Setup(r => r.SoftDeleteAsync(It.IsAny<BookCopy>())).Returns(Task.CompletedTask);

            // Act
            var result = await _bookService.DeleteBookCopy(1);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().BeTrue();
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        
        

        [Fact]
        public async Task ReturnBook_WithValidBorrowedBookId_ReturnsBorrowedBookDto()
        {
            // Arrange
            var borrowedBook = new BorrowedBook { BorrowedBookId = 1, BookCopyId = 1, BookCopy = new BookCopy { BookCopyId = 1, IsAvailable = false }, BorrowDate = DateTime.Now };
            _mockBorrowBookRepo.Setup(r => r.Get(It.IsAny<Expression<Func<BorrowedBook, bool>>>())).Returns(new List<BorrowedBook> { borrowedBook }.AsQueryable());
            _mockBookCopyRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(borrowedBook.BookCopy);
            _mockBorrowBookRepo.Setup(r => r.InsertOrUpdateAsync(It.IsAny<int>(), It.IsAny<BorrowedBook>())).ReturnsAsync(borrowedBook);
            _mockBookCopyRepo.Setup(r => r.InsertOrUpdateAsync(It.IsAny<int>(), It.IsAny<BookCopy>())).ReturnsAsync(borrowedBook.BookCopy);

            // Act
            var result = await _bookService.ReturnBook(1);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().NotBeNull();
            result.Data?.BorrowedBookId.Should().Be(1);
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }
    }
}
