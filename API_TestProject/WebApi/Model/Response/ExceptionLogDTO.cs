using System.ComponentModel.DataAnnotations;

namespace API_TestProject.WebApi.Model.Response
{
    // Exception Example:
    // { "type": "Exception", "id": "638136064187111634", "data": { "message": "Internal server error ID = 638136064187111634"} }`

    public class ExceptionLogDTO
    {
        public string type { get; set; }
        public Guid id { get; set; }
        public ExceptionLogDataDTO data { get; set; }
    }

    public class ExceptionLogDataDTO
    {
        public string message { get; set; }
    }
}
