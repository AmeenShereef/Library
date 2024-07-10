using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookLibrary.Data.Migrations
{
    /// <inheritdoc />
    public partial class BookMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    BookId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Author = table.Column<string>(type: "TEXT", nullable: true),
                    Genre = table.Column<string>(type: "TEXT", nullable: true),
                    ISBN = table.Column<string>(type: "TEXT", nullable: true),
                    PublicationYear = table.Column<int>(type: "INTEGER", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatorUserId = table.Column<long>(type: "INTEGER", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.BookId);
                });

            migrationBuilder.CreateTable(
                name: "BookCopys",
                columns: table => new
                {
                    BookCopyId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BookId = table.Column<int>(type: "INTEGER", nullable: false),
                    CopyNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    AcquisitionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsAvailable = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatorUserId = table.Column<long>(type: "INTEGER", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookCopys", x => x.BookCopyId);
                    table.ForeignKey(
                        name: "FK_BookCopys_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "BookId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BorrowedBooks",
                columns: table => new
                {
                    BorrowedBookId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BookCopyId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    BorrowDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ReturnDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatorUserId = table.Column<long>(type: "INTEGER", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BorrowedBooks", x => x.BorrowedBookId);
                    table.ForeignKey(
                        name: "FK_BorrowedBooks_BookCopys_BookCopyId",
                        column: x => x.BookCopyId,
                        principalTable: "BookCopys",
                        principalColumn: "BookCopyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BorrowedBooks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "BookId", "Author", "CreationTime", "CreatorUserId", "Genre", "ISBN", "LastModificationTime", "LastModifierUserId", "PublicationYear", "Title" },
                values: new object[,]
                {
                    { 1, "Harper Lee", null, null, "Fiction", "9780446310789", null, null, 1960, "To Kill a Mockingbird" },
                    { 2, "George Orwell", null, null, "Science Fiction", "9780451524935", null, null, 1949, "1984" },
                    { 3, "Jane Austen", null, null, "Classic", "9780141439518", null, null, 1813, "Pride and Prejudice" },
                    { 4, "J.D. Salinger", null, null, "Fiction", "9780316769174", null, null, 1951, "The Catcher in the Rye" },
                    { 5, "F. Scott Fitzgerald", null, null, "Fiction", "9780743273565", null, null, 1925, "The Great Gatsby" }
                });

            migrationBuilder.InsertData(
                table: "BookCopys",
                columns: new[] { "BookCopyId", "AcquisitionDate", "BookId", "CopyNumber", "CreationTime", "CreatorUserId", "IsAvailable", "LastModificationTime", "LastModifierUserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2020, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, null, null, true, null, null },
                    { 2, new DateTime(2020, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, null, null, true, null, null },
                    { 3, new DateTime(2020, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1, null, null, true, null, null },
                    { 4, new DateTime(2020, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 2, null, null, true, null, null },
                    { 5, new DateTime(2020, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 1, null, null, true, null, null },
                    { 6, new DateTime(2020, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 1, null, null, true, null, null },
                    { 7, new DateTime(2020, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 1, null, null, true, null, null },
                    { 8, new DateTime(2020, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 2, null, null, true, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookCopys_BookId",
                table: "BookCopys",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowedBooks_BookCopyId",
                table: "BorrowedBooks",
                column: "BookCopyId");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowedBooks_UserId",
                table: "BorrowedBooks",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BorrowedBooks");

            migrationBuilder.DropTable(
                name: "BookCopys");

            migrationBuilder.DropTable(
                name: "Books");
        }
    }
}
