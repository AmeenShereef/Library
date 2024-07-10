using BookLibrary.Data.Entities;
using BookLibrary.Data.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BookLibrary.Data
{
    public class APIContext : DbContext
    {
        public const string Name = "BookLibrary";
        public virtual DbSet<ExceptionLog> ExceptionLogs { get; set; } = default!;
        public virtual DbSet<APILog> APILogs { get; set; } = default!;
        public virtual DbSet<User> Users { get; set; } = default!;
        public virtual DbSet<Role> Roles { get; set; } = default!;
        public virtual DbSet<Book> Books { get; set; } = default!;
        public virtual DbSet<BookCopy> BookCopys { get; set; } = default!;
        public virtual DbSet<BorrowedBook> BorrowedBooks { get; set; } = default!;

        protected readonly IConfiguration? Configuration;

        public APIContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.LoadConfigurationsFromAssembly();

            // Seed roles data
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, Name = "Admin" },
                new Role { RoleId = 2, Name = "User" }
            );
            // Seed Books
            modelBuilder.Entity<Book>().HasData(
                new Book { BookId = 1, Title = "To Kill a Mockingbird", Author = "Harper Lee", Genre = "Fiction", ISBN = "9780446310789", PublicationYear = 1960 },
                new Book { BookId = 2, Title = "1984", Author = "George Orwell", Genre = "Science Fiction", ISBN = "9780451524935", PublicationYear = 1949 },
                new Book { BookId = 3, Title = "Pride and Prejudice", Author = "Jane Austen", Genre = "Classic", ISBN = "9780141439518", PublicationYear = 1813 },
                new Book { BookId = 4, Title = "The Catcher in the Rye", Author = "J.D. Salinger", Genre = "Fiction", ISBN = "9780316769174", PublicationYear = 1951 },
                new Book { BookId = 5, Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", Genre = "Fiction", ISBN = "9780743273565", PublicationYear = 1925 }
            );

            // Seed BookCopies
            modelBuilder.Entity<BookCopy>().HasData(
                new BookCopy { BookCopyId = 1, BookId = 1, CopyNumber = 1, IsAvailable = true, AcquisitionDate = new DateTime(2020, 1, 15) },
                new BookCopy { BookCopyId = 2, BookId = 1, CopyNumber = 2, IsAvailable = true, AcquisitionDate = new DateTime(2020, 1, 15) },
                new BookCopy { BookCopyId = 3, BookId = 2, CopyNumber = 1, IsAvailable = true, AcquisitionDate = new DateTime(2020, 2, 20) },
                new BookCopy { BookCopyId = 4, BookId = 2, CopyNumber = 2, IsAvailable = true, AcquisitionDate = new DateTime(2020, 2, 20) },
                new BookCopy { BookCopyId = 5, BookId = 3, CopyNumber = 1, IsAvailable = true, AcquisitionDate = new DateTime(2020, 3, 10) },
                new BookCopy { BookCopyId = 6, BookId = 4, CopyNumber = 1, IsAvailable = true, AcquisitionDate = new DateTime(2020, 4, 5) },
                new BookCopy { BookCopyId = 7, BookId = 5, CopyNumber = 1, IsAvailable = true, AcquisitionDate = new DateTime(2020, 5, 1) },
                new BookCopy { BookCopyId = 8, BookId = 5, CopyNumber = 2, IsAvailable = true, AcquisitionDate = new DateTime(2020, 5, 1) }
                );
        }
    }
}