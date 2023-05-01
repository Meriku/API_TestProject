using API_TestProject.Core.Model;
using API_TestProject.DataBase;
using API_TestProject.DataBase.Model;
using API_TestProject.WebApi.Model.Request;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace API_TestProject.Core
{
    public class JournalService
    {
        private readonly ILogger<JournalService> _logger;
        private readonly APIContext _context;

        public JournalService(APIContext context, ILogger<JournalService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<EventLogList> GetRange(int skip, int take, FilterDTO filter)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            GetFilterDates(filter, out var filterFrom, out var filterTo);

            var requestHash = $"{skip}_{take}_{filterFrom}_{filterTo}";
            var eventLogListIds = CacheManager.GetValue<EventLogs>(stringKey: requestHash);

            var eventLogItems = new List<ExceptionLog>(0);
            if (eventLogListIds == null || eventLogListIds.Value.Length != take)
            {
                var message = eventLogListIds == null ? "EventLogs was not found in the cache." : "There is a possibility that cache is outdated.";
                _logger.LogInformation($"{message} Starting request to the DataBase.");
                eventLogItems = await GetItemsFromDB(skip, take, filterFrom, filterTo);

                if (eventLogItems.Count > 0 && eventLogListIds?.Value.Length != eventLogItems.Count)
                {
                    var itemIds = new int[eventLogItems.Count];
                    var keysInCache = CacheManager.GetCachedKeys<ExceptionLog, HashSet<int>>();
                    for (var i = 0; i < eventLogItems.Count; i++)
                    {
                        var eventLog = eventLogItems[i];
                        itemIds[i] = eventLog.ExceptionLogId;

                        if (!keysInCache.Contains(eventLog.ExceptionLogId))
                            CacheManager.SetValue<ExceptionLog>(eventLog, numberKey: eventLog.ExceptionLogId);
                    }
                    CacheManager.SetValue<EventLogs>(new EventLogs() { Value = itemIds }, stringKey: requestHash);
                }
            }
            else
            {
                eventLogItems = new List<ExceptionLog>(eventLogListIds.Value.Length);
                for (var i = 0; i < eventLogListIds.Value.Length; i++)
                {
                    var id = eventLogListIds.Value[i];
                    var exceptionLogItem = CacheManager.GetValue<ExceptionLog>(numberKey: id);
                    if (exceptionLogItem == null) // This exception should never be thrown
                    { throw new ArgumentException("Unexpected error occurred while retrieving data from cache."); }
                    
                    eventLogItems.Add(exceptionLogItem);
                }
            }

            var count = _context.ExceptionLogs.Count();

            eventLogItems = ApplySearchFilter(eventLogItems, filter.Search);

            var result = new EventLogList()
            {
                Skip = skip,
                Count = count,
                Items = eventLogItems
            };

            stopwatch.Stop();
            var cacheOrDataBase = eventLogListIds == null ? "Data retrieved from DataBase." : "Data retrieved from Cache.";
            _logger.LogInformation($"JournalService: GetRange request done. Took: {stopwatch.ElapsedMilliseconds} milliseconds. Items Count: {eventLogItems.Count}. " +
                $"{cacheOrDataBase} Filtering: From: {filterFrom != null} To: {filterTo != null} Search: {!string.IsNullOrWhiteSpace(filter.Search)}.");

            return result;
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

        private List<ExceptionLog> ApplySearchFilter(List<ExceptionLog> items, string requestedSubString)
        {
            if (string.IsNullOrWhiteSpace(requestedSubString))
            { return items; }

            return items.Where(i => i.EventId.ToString().Contains(requestedSubString)).ToList();
        }

        private async Task<List<ExceptionLog>> GetItemsFromDB(int skip, int take, DateTime? filterFrom, DateTime? filterTo)
        {
            if (filterFrom == null && filterTo == null)
            {
                return await _context.ExceptionLogs.Skip(skip).Take(take).ToListAsync();
            }
            else if (filterFrom != null && filterTo != null)
            {
                return await _context.ExceptionLogs.Where(x => x.Timestamp >= filterFrom && x.Timestamp <= filterTo).Skip(skip).Take(take).ToListAsync();
            }
            else if (filterTo != null)
            {
                return await _context.ExceptionLogs.Where(x => x.Timestamp <= filterTo).Skip(skip).Take(take).ToListAsync();
            }
            else if (filterFrom != null)
            {
                return await _context.ExceptionLogs.Where(x => x.Timestamp >= filterFrom).Skip(skip).Take(take).ToListAsync();
            }
            throw new ArgumentException("Unexpected error occurred while retrieving data from database.");
        }

        private void GetFilterDates(FilterDTO filter, out DateTime? from, out DateTime? to)
        {
            from = null;
            to = null;
            if (filter.From != null)
            {
                if (DateTime.TryParse(filter.From, out var resultFrom))
                {
                    from = resultFrom.ToUniversalTime();
                }
                else
                {
                    throw new ArgumentException($"Filter value: 'From' was in incorrect format - {filter.From}");
                }
            }
            if (filter.To != null)
            {
                if (DateTime.TryParse(filter.To, out var resultTo))
                {
                    to = resultTo.ToUniversalTime();
                }
                else
                {
                    throw new ArgumentException($"Filter value: 'To' was in incorrect format - {filter.To}");
                }
            }
        }

    }
}
