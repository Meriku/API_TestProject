using System.ComponentModel.DataAnnotations;

namespace API_TestProject.DataBase.Model
{
    public class ExceptionLog
    {
        [Key]
        public int ExceptionLogId { get; set; }
        public string Type { get; set; }
        public Guid EventId { get; set; }
        public DateTime Timestamp { get; set; }
        public string QueryParameters { get; set; }
        public string BodyParameters { get; set; }
        public string StackTrace { get; set; }
        public string Message { get; set; }
    }
}
