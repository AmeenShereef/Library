using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibrary.Data.Entities
{
    public class BorrowedBook : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BorrowedBookId { get; set; }

        [ForeignKey("BookCopyId")]
        public int BookCopyId { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }

        public DateTime BorrowDate { get; set; }

        public DateTime? DueDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        public BookCopy BookCopy { get; set; } = default!;
        public User User { get; set; } = default!;
    }
}
