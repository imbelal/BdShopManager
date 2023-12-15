namespace Domain.Entities
{
    public class ErrorLog
    {
        public int Id { get; set; }
        public DateTime CreatedDateUtc { get; set; }
        public string Message { get; set; }
        public string Level { get; set; }
        public string Exception { get; set; }
        public string StackTrace { get; set; }
        public string Logger { get; set; }
    }
}
