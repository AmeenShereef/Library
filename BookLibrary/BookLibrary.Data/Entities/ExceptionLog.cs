namespace BookLibrary.Data.Entities
{
    public class ExceptionLog
    {
        public int Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public string? Source { get; set; }

        public string? TargetSite { get; set; }

        public string? Message { get; set; }

        public string? StackTrace { get; set; }

        public string? Path { get; set; }

        public string? QueryString { get; set; }

        public string? Body { get; set; }

        public string? UserAgent { get; set; }
    }
}
