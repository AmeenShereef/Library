using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibrary.Models
{
    public class BookDto
    {
        public int BookId { get; set; }
        public string Title { get; set; } 
        public string? Author { get; set; }
        public string? Genre { get; set; }
        public string? ISBN { get; set; }
        public int? PublicationYear { get; set; }
        public bool? IsAvailable { get; set; }
        public int? AvailableBookCopyId {  get; set; }
    }
}
