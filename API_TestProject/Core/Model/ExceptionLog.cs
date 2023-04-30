using System.ComponentModel.DataAnnotations;
using API_TestProject.DataBase.Model;
using API_TestProject.WebApi.Model.Response;

namespace API_TestProject.Core.Model
{
    public class ExceptionLogExtended : ExceptionLog
    {
    }

    public class EventLogs
    {
        public int[] Value { get; set; }
    }

    public class EventLogList
    {
        public int Skip { get; set; }
        public int Count { get; set; }
        public ICollection<ExceptionLog> Items { get; set; }
    }
}
