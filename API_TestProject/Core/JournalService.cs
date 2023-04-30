using API_TestProject.Core.Model;
using API_TestProject.DataBase;
using API_TestProject.DataBase.Model;
using API_TestProject.WebApi.Model.Request;
using API_TestProject.WebApi.Model.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_TestProject.Core
{
    public class JournalService
    {
        private readonly ILogger<TreeService> _logger;
        private readonly APIContext _context;

        public JournalService(APIContext context, ILogger<TreeService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<EventLogListDTO> GetRange(int skip, int take, FilterDTO filter)
        {
            
        }

        public async Task<ExceptionLog> GetSingle(int id)
        {
            var exceptionLogItem = CacheManager.GetValue<ExceptionLog>(numberKey: id);

            if (exceptionLogItem == null)
            {
                _logger.LogInformation($"ExceptionLog was not found in the cache. Starting request to the DataBase.");
                var eventLogItemDB = await _context.ExceptionLogs.FirstOrDefaultAsync(x => x.ExceptionLogId == id);
                if (eventLogItemDB == null)
                {
                    throw new SecureException($"EventLogItem with Id {id} doesn't exist.");
                }
                CacheManager.SetValue<ExceptionLog>(eventLogItemDB, numberKey: id);
                exceptionLogItem = eventLogItemDB;
            }

            return exceptionLogItem;
        }





    }
}
