using BookLibrary.Data.Entities;
using BookLibrary.Models;
using Mapster;

namespace BookLibrary.Business.Mapper
{
    public class GeneralMapper : IRegister
    {
                
        public void Register(TypeAdapterConfig config)   
        {

            TypeAdapterConfig<User, UserDto>
                .NewConfig()
                .Map(dest => dest.Role, src => src.Role != null ? src.Role.Name : null);

            
            TypeAdapterConfig<BookCopy, BookDto>
                .NewConfig()
                .Map(dest => dest, src => src.Book)
                .Map(dest => dest.IsAvailable, src => src.IsAvailable)
                .Map(dest => dest.AvailableBookCopyId, src => src.IsAvailable ? src.BookCopyId : (int?)null)
                .MaxDepth(2);

            TypeAdapterConfig<BorrowedBook, BorrowedBookDto>
                .NewConfig()
                .Map(dest => dest.BookId, src => src.BookCopy.BookId)
                .Map(dest => dest.Title, src => src.BookCopy.Book.Title)
                .Map(dest => dest.Author, src => src.BookCopy.Book.Author)
                .Map(dest => dest.Genre, src => src.BookCopy.Book.Genre)
                .Map(dest => dest.ISBN, src => src.BookCopy.Book.ISBN)
                .MaxDepth(3);

            TypeAdapterConfig<Book, BookDto>
                .NewConfig()
                .Map(dest => dest.IsAvailable, src => src.BookCopies.Any())
                .Map(dest => dest.AvailableBookCopyId, src => src.BookCopies.Select(bc => (int?)bc.BookCopyId).FirstOrDefault())
                .MaxDepth(2);

        }
    }
}
