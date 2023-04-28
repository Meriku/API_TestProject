using System.ComponentModel.DataAnnotations;

namespace API_TestProject.Model.Internal
{
    public class ExceptionLog
    {
        [Key]
        public int Id { get; set; }
        public Guid EventId { get; set; }
        public DateTime Timestamp { get; set; }
        public string QueryParameters { get; set; }
        public string BodyParameters { get; set; }
        public string StackTrace { get; set; }
    }
}
