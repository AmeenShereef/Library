using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BookLibrary.Data.Entities
{

    public class BookCopy:AuditableEntity
    {
        public int BookCopyId { get; set; }
        public int BookId { get; set; }
        public int CopyNumber { get; set; }       
        public DateTime AcquisitionDate { get; set; }
        public bool IsAvailable { get; set; }

        public Book Book { get; set; } = default!;
       
    }
   
}
