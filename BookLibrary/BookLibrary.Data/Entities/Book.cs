using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibrary.Data.Entities
{
    public class Book : AuditableEntity
    {
        public int BookId { get; set; }
        public string Title { get; set; } = default!;
        public string? Author { get; set; }
        public string? Genre { get; set; }
        public string? ISBN { get; set; }
        public int? PublicationYear { get; set; }

        public ICollection<BookCopy> BookCopies { get; set; } = default!;
    }
    
}
