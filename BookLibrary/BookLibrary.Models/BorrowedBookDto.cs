using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibrary.Models
{
    public class BorrowedBookDto
    {
        public int BorrowedBookId { get; set; }
       
        public int BookCopyId { get; set; }
       
        public int UserId { get; set; }

        public DateTime BorrowDate { get; set; }

        public DateTime? DueDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        public BookCopyDto BookCopy { get; set; } = default!;
    }
}
