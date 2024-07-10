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
                .Map(dest => dest.BookCopy, src => src.BookCopy)
                .MaxDepth(3);

            TypeAdapterConfig<Book, BookDto>
                .NewConfig()
                .Map(dest => dest.IsAvailable, src => src.BookCopies.Any())
                .Map(dest => dest.AvailableBookCopyId, src => src.BookCopies.Select(bc => (int?)bc.BookCopyId).FirstOrDefault())
                .MaxDepth(2);

        }
    }
}
