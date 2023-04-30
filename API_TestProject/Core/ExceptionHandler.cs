using API_TestProject.DataBase;
using API_TestProject.DataBase.Model;
using API_TestProject.WebApi.Model.Response;
using AutoMapper;
using System.Text;
using System.Text.Json;

namespace API_TestProject.Core
{
    internal class ExceptionHandler
    {
        private readonly ILogger<ExceptionHandler> _logger;
        private readonly APIContext _context;
        private readonly IMapper _mapper;

        public ExceptionHandler(APIContext context, ILogger<ExceptionHandler> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// The method HandleExceptionAsync receives an HttpContext and an Exception as input parameters, and logs the details of the exception into a database table called ExceptionLogs. 
        /// If an exception occurs while adding the ExceptionLog instance to the table, the method logs the error message and the exception to the file.
        /// </summary>
        internal async Task<string> HandleExceptionAsync(HttpContext context, Exception exception)
        {
            string? queryParameters = string.Empty;
            string? bodyParameters = string.Empty;
            string? stackTrace = string.Empty;
            string? message = string.Empty;
            string? exceptionType = string.Empty;

            if (context != null && context.Request != null)
            {
                try
                {
                    queryParameters = string.Join("; ", context.Request.Query.Select(x => x.Key + ": " + x.Value));

                    var bodyStream = new StreamReader(context.Request.Body);
                    bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
                    bodyParameters = bodyStream.ReadToEnd();
                }
                catch { }
  
            }
            if (exception != null)
            {
                if (exception.StackTrace != null)
                { stackTrace = exception.StackTrace; }

                exceptionType = exception.GetType().Name;
                message = exception.Message;
            }

            var exceptionLog = new ExceptionLog()
            {
                Type = exceptionType,
                EventId = Guid.NewGuid(),
                Timestamp = DateTime.Now.ToUniversalTime(),
                QueryParameters = queryParameters,
                BodyParameters = bodyParameters,
                StackTrace = stackTrace,
                Message = message
            };

            var exceptionLogDTO = _mapper.Map<ExceptionLog, ExceptionLogDTO>(exceptionLog);
            var jsonExceptionLogDTO = JsonSerializer.Serialize(exceptionLogDTO);

            try
            {
                _context.ExceptionLogs.Add(exceptionLog);
                await _context.SaveChangesAsync();
                _logger.LogError(jsonExceptionLogDTO);
            }
            catch(Exception ex)
            {
                _logger.LogError(jsonExceptionLogDTO);
                _logger.LogError("Unexpected error while trying to save error message in DataBase: " + ex.Message);
            }

            return jsonExceptionLogDTO;
        }
    }
}
