using System.ComponentModel.DataAnnotations;
using API_TestProject.DataBase.Model;
using API_TestProject.WebApi.Model.Response;

namespace API_TestProject.Core.Model
{
    public class ExceptionLogExtended : ExceptionLog, ICacheableValue
    {
        public ExceptionLogExtended(ExceptionLog log) 
        {
            base.ExceptionLogId = log.ExceptionLogId;
            base.Type = log.Type;
            base.EventId = log.EventId;
            base.Timestamp = log.Timestamp;
            base.QueryParameters = log.QueryParameters;
            base.BodyParameters = log.BodyParameters;
            base.StackTrace = log.StackTrace;
            base.Message = log.Message;

            RequestsCount = 0;
        }

        public int RequestsCount { get; set; }
    }

    public class EventLogs : ICacheableValue
    {
        public EventLogs()
        {
            RequestsCount = 0;
        }

        public int[] Value { get; set; }
        public int RequestsCount { get; set; }
    }

    public class EventLogList
    {
        public int Skip { get; set; }
        public int Count { get; set; }
        public ICollection<ExceptionLog> Items { get; set; }
    }
}
