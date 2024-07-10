using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibrary.Models
{
    public class BookCopyDto
    {
        public int BookCopyId { get; set; }       
        public int CopyNumber { get; set; }
        public DateTime AcquisitionDate { get; set; }
        public bool IsAvailable { get; set; }       
    }
}
