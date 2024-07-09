using System.ComponentModel.DataAnnotations.Schema;

namespace BookLibrary.Data.Entities
{

    [Table("APILog")]
    public class APILog
    {
        public int Id { get; set; }
               
        public string? Path { get; set; }

        public string? QueryString { get; set; }

        public string? Method { get; set; }

        public string? UserAgent { get; set; }

        public string? Host { get; set; }

        public long? UserId { get; set; }
     
        public string? Headers { get; set; }
    }
}
