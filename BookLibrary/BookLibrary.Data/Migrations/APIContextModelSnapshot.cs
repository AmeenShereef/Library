﻿// <auto-generated />
using System;
using BookLibrary.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BookLibrary.Data.Migrations
{
    [DbContext(typeof(APIContext))]
    partial class APIContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.6");

            modelBuilder.Entity("BookLibrary.Data.Entities.APILog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Headers")
                        .HasColumnType("TEXT");

                    b.Property<string>("Host")
                        .HasColumnType("TEXT");

                    b.Property<string>("Method")
                        .HasColumnType("TEXT");

                    b.Property<string>("Path")
                        .HasColumnType("TEXT");

                    b.Property<string>("QueryString")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserAgent")
                        .HasColumnType("TEXT");

                    b.Property<long?>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("APILog");
                });

            modelBuilder.Entity("BookLibrary.Data.Entities.Book", b =>
                {
                    b.Property<int>("BookId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Author")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreationTime")
                        .HasColumnType("TEXT");

                    b.Property<long?>("CreatorUserId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Genre")
                        .HasColumnType("TEXT");

                    b.Property<string>("ISBN")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("LastModificationTime")
                        .HasColumnType("TEXT");

                    b.Property<long?>("LastModifierUserId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("PublicationYear")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("BookId");

                    b.ToTable("Books");

                    b.HasData(
                        new
                        {
                            BookId = 1,
                            Author = "Harper Lee",
                            Genre = "Fiction",
                            ISBN = "9780446310789",
                            PublicationYear = 1960,
                            Title = "To Kill a Mockingbird"
                        },
                        new
                        {
                            BookId = 2,
                            Author = "George Orwell",
                            Genre = "Science Fiction",
                            ISBN = "9780451524935",
                            PublicationYear = 1949,
                            Title = "1984"
                        },
                        new
                        {
                            BookId = 3,
                            Author = "Jane Austen",
                            Genre = "Classic",
                            ISBN = "9780141439518",
                            PublicationYear = 1813,
                            Title = "Pride and Prejudice"
                        },
                        new
                        {
                            BookId = 4,
                            Author = "J.D. Salinger",
                            Genre = "Fiction",
                            ISBN = "9780316769174",
                            PublicationYear = 1951,
                            Title = "The Catcher in the Rye"
                        },
                        new
                        {
                            BookId = 5,
                            Author = "F. Scott Fitzgerald",
                            Genre = "Fiction",
                            ISBN = "9780743273565",
                            PublicationYear = 1925,
                            Title = "The Great Gatsby"
                        });
                });

            modelBuilder.Entity("BookLibrary.Data.Entities.BookCopy", b =>
                {
                    b.Property<int>("BookCopyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("AcquisitionDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("BookId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CopyNumber")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("CreationTime")
                        .HasColumnType("TEXT");

                    b.Property<long?>("CreatorUserId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastModificationTime")
                        .HasColumnType("TEXT");

                    b.Property<long?>("LastModifierUserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("BookCopyId");

                    b.HasIndex("BookId");

                    b.ToTable("BookCopys");

                    b.HasData(
                        new
                        {
                            BookCopyId = 1,
                            AcquisitionDate = new DateTime(2020, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            BookId = 1,
                            CopyNumber = 1,
                            IsAvailable = true
                        },
                        new
                        {
                            BookCopyId = 2,
                            AcquisitionDate = new DateTime(2020, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            BookId = 1,
                            CopyNumber = 2,
                            IsAvailable = true
                        },
                        new
                        {
                            BookCopyId = 3,
                            AcquisitionDate = new DateTime(2020, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            BookId = 2,
                            CopyNumber = 1,
                            IsAvailable = true
                        },
                        new
                        {
                            BookCopyId = 4,
                            AcquisitionDate = new DateTime(2020, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            BookId = 2,
                            CopyNumber = 2,
                            IsAvailable = true
                        },
                        new
                        {
                            BookCopyId = 5,
                            AcquisitionDate = new DateTime(2020, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            BookId = 3,
                            CopyNumber = 1,
                            IsAvailable = true
                        },
                        new
                        {
                            BookCopyId = 6,
                            AcquisitionDate = new DateTime(2020, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            BookId = 4,
                            CopyNumber = 1,
                            IsAvailable = true
                        },
                        new
                        {
                            BookCopyId = 7,
                            AcquisitionDate = new DateTime(2020, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            BookId = 5,
                            CopyNumber = 1,
                            IsAvailable = true
                        },
                        new
                        {
                            BookCopyId = 8,
                            AcquisitionDate = new DateTime(2020, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            BookId = 5,
                            CopyNumber = 2,
                            IsAvailable = true
                        });
                });

            modelBuilder.Entity("BookLibrary.Data.Entities.BorrowedBook", b =>
                {
                    b.Property<int>("BorrowedBookId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BookCopyId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("BorrowDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreationTime")
                        .HasColumnType("TEXT");

                    b.Property<long?>("CreatorUserId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("DueDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("LastModificationTime")
                        .HasColumnType("TEXT");

                    b.Property<long?>("LastModifierUserId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("ReturnDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("BorrowedBookId");

                    b.HasIndex("BookCopyId");

                    b.HasIndex("UserId");

                    b.ToTable("BorrowedBooks");
                });

            modelBuilder.Entity("BookLibrary.Data.Entities.ExceptionLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Body")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("TEXT");

                    b.Property<string>("Message")
                        .HasColumnType("TEXT");

                    b.Property<string>("Path")
                        .HasColumnType("TEXT");

                    b.Property<string>("QueryString")
                        .HasColumnType("TEXT");

                    b.Property<string>("Source")
                        .HasColumnType("TEXT");

                    b.Property<string>("StackTrace")
                        .HasColumnType("TEXT");

                    b.Property<string>("TargetSite")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserAgent")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ExceptionLogs");
                });

            modelBuilder.Entity("BookLibrary.Data.Entities.Role", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("RoleId");

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            RoleId = 1,
                            Name = "Admin"
                        },
                        new
                        {
                            RoleId = 2,
                            Name = "User"
                        });
                });

            modelBuilder.Entity("BookLibrary.Data.Entities.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AccessToken")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreationTime")
                        .HasColumnType("TEXT");

                    b.Property<long?>("CreatorUserId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("DisplayName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastLoginTime")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("LastModificationTime")
                        .HasColumnType("TEXT");

                    b.Property<long?>("LastModifierUserId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Password")
                        .HasColumnType("TEXT");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("TEXT");

                    b.Property<string>("RegistrationCode")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("RegistrationCodeTime")
                        .HasColumnType("TEXT");

                    b.Property<int>("RoleId")
                        .HasColumnType("INTEGER");

                    b.HasKey("UserId");

                    b.HasIndex("RoleId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("BookLibrary.Data.Entities.BookCopy", b =>
                {
                    b.HasOne("BookLibrary.Data.Entities.Book", "Book")
                        .WithMany("Copies")
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Book");
                });

            modelBuilder.Entity("BookLibrary.Data.Entities.BorrowedBook", b =>
                {
                    b.HasOne("BookLibrary.Data.Entities.BookCopy", "BookCopy")
                        .WithMany("BorrowedBooks")
                        .HasForeignKey("BookCopyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookLibrary.Data.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BookCopy");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BookLibrary.Data.Entities.User", b =>
                {
                    b.HasOne("BookLibrary.Data.Entities.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("BookLibrary.Data.Entities.Book", b =>
                {
                    b.Navigation("Copies");
                });

            modelBuilder.Entity("BookLibrary.Data.Entities.BookCopy", b =>
                {
                    b.Navigation("BorrowedBooks");
                });
#pragma warning restore 612, 618
        }
    }
}
