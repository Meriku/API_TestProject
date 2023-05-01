using API_TestProject.Core.Model;
using API_TestProject.DataBase.Model;

namespace API_TestProject.Core
{
    public static class CacheManager
    {
        internal static readonly int MINUTES_TO_UPDATE = 5;
        private static readonly int MAX_TREES_COUNT = 30;
        private static readonly int MAX_EVENT_LOG_SEARCHES_COUNT = 100;
        private static readonly int MAX_EXCEPTION_LOG_ITEMS_COUNT = 1000;

        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        private static bool _isCacheInitialized = false;

        private static Dictionary<string, CachedItem<TreeExtended>> CachedTrees;
        private static Dictionary<int, ExceptionLogExtended> CachedExceptionLogs;
        private static Dictionary<string, EventLogs> CachedEventLogsSearch;

        private static void InitializeCache()
        {
            CachedTrees = new Dictionary<string, CachedItem<TreeExtended>>();
            CachedExceptionLogs = new Dictionary<int, ExceptionLogExtended>();
            CachedEventLogsSearch = new Dictionary<string, EventLogs>();

            _isCacheInitialized = true;
        }

        private static void CheckMemory()
        {
            if (CachedTrees.Count > MAX_TREES_COUNT || CachedEventLogsSearch.Count > MAX_EVENT_LOG_SEARCHES_COUNT || CachedExceptionLogs.Count > MAX_EXCEPTION_LOG_ITEMS_COUNT)
            {
                Task.Run(() => { ClearMemory(); });
            }
        }
        private static void ClearMemory()
        {
            _semaphore.Wait();
            if (CachedTrees.Count > MAX_TREES_COUNT)
            {
                var averageRequestsCount = CachedTrees.Average(x => x.Value.RequestsCount);
                var keysToRemove = CachedTrees.Where(x => x.Value.RequestsCount <= averageRequestsCount).Select(x => x.Key);
                foreach (var key in keysToRemove)
                {
                    CachedTrees.Remove(key);
                }
            }
            if (CachedEventLogsSearch.Count > MAX_EVENT_LOG_SEARCHES_COUNT)
            {
                var averageRequestsCount = CachedEventLogsSearch.Average(x => x.Value.RequestsCount);
                var keysToRemove = CachedEventLogsSearch.Where(x => x.Value.RequestsCount <= averageRequestsCount).Select(x => x.Key);
                foreach (var key in keysToRemove)
                {
                    CachedEventLogsSearch.Remove(key);
                }
            }
            if (CachedExceptionLogs.Count > MAX_EXCEPTION_LOG_ITEMS_COUNT)
            {
                var averageRequestsCount = CachedExceptionLogs.Average(x => x.Value.RequestsCount);
                var keysToRemove = CachedExceptionLogs.Where(x => x.Value.RequestsCount <= averageRequestsCount).Select(x => x.Key);
                foreach (var key in keysToRemove)
                {
                    CachedExceptionLogs.Remove(key);
                }

                var searchKeysToRemove = CachedEventLogsSearch.Where(x => x.Value.Value.Any(x => keysToRemove.Contains(x))).Select(x => x.Key);
                foreach (var key in searchKeysToRemove)
                {
                    CachedEventLogsSearch.Remove(key);
                }
            }
            _semaphore.Release();
        }

        /// <summary>
        /// This is a generic method that returns a cached HashSet of keys for a given input type CachedValueT. 
        /// Performs a switch statement based on the name of the input type. 
        /// If the input type doesn't match any of the predefined types, it also throws an ArgumentException.
        /// </summary>
        /// <typeparam name="CachedValueT"> Type of Cached Value </typeparam>
        /// <typeparam name="HashSetT"> Type of T in HashSet </typeparam>
        public static HashSetT GetCachedKeys<CachedValueT, HashSetT>()
        {
            if (!_isCacheInitialized)
            { InitializeCache(); }

            _semaphore.Wait();
            try
            {
                return GetCachedKeysInternal<CachedValueT, HashSetT>();
            }
            catch
            {
                throw;
            }
            finally
            {
                _semaphore.Release();
            }

        }
        private static HashSetT GetCachedKeysInternal<CachedValueT, HashSetT>()
        {
            switch (typeof(CachedValueT).Name)
            {
                case nameof(ExceptionLog):
                    if (CachedExceptionLogs.Keys.ToHashSet() is HashSetT genericResultExceptionLog)
                        return genericResultExceptionLog;
                    throw new ArgumentException();
                case nameof(TreeExtended):
                    if (CachedTrees.Keys.ToHashSet() is HashSetT genericResultTreeExtended)
                        return genericResultTreeExtended;
                    throw new ArgumentException();
                case nameof(EventLogs):
                    if (CachedEventLogsSearch.Keys.ToHashSet() is HashSetT genericResultEventLogs)
                        return genericResultEventLogs;
                    throw new ArgumentException();
                default:
                    throw new ArgumentException();
            }
        }
        public static CachedValueT? GetValue<CachedValueT>(string? stringKey = null)
        {
            if (!_isCacheInitialized)
            { InitializeCache(); }

            _semaphore.Wait();
            try
            {
                return GetValueInternal<CachedValueT>(stringKey);
            }
            catch
            {
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        private static CachedValueT? GetValueInternal<CachedValueT>(string? stringKey = null)
        {
            switch (typeof(CachedValueT).Name)
            {
                case nameof(TreeExtended):
                    if (stringKey == null)
                        throw new ArgumentNullException(nameof(stringKey), "Key is required to retrieve Tree from the cache.");
                    if (CachedTrees.ContainsKey(stringKey) && CachedTrees[stringKey].Item is CachedValueT genericResultTreeExtended)
                    {
                        CachedTrees[stringKey].RequestsCount++;
                        return genericResultTreeExtended;
                    }
                    return default;
                case nameof(EventLogs):
                    if (stringKey == null)
                        throw new ArgumentNullException(nameof(stringKey), "Key is required to retrieve EventLogs from the cache.");
                    if (CachedEventLogsSearch.ContainsKey(stringKey) && CachedEventLogsSearch[stringKey] is CachedValueT genericResultEventLogs)
                    {
                        CachedEventLogsSearch[stringKey].RequestsCount++;
                        return genericResultEventLogs;
                    }
                    return default;
                default:
                    throw new ArgumentException();
            }    
        }
        public static CachedValueT? GetValue<CachedValueT>(int? numberKey = null)
        {
            if (!_isCacheInitialized)
            { InitializeCache(); }

            _semaphore.Wait();
            try
            {
                return GetValueInternal<CachedValueT>(numberKey);
            }
            catch
            {
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        private static CachedValueT? GetValueInternal<CachedValueT>(int? numberKey = null)
        {
            switch (typeof(CachedValueT).Name)
            {
                case nameof(ExceptionLog):
                    if (numberKey == null)
                        throw new ArgumentNullException(nameof(numberKey), "Key is required to retrieve ExceptionLog from the cache.");
                    if (CachedExceptionLogs.ContainsKey((int)numberKey) && CachedExceptionLogs[(int)numberKey] is CachedValueT genericResultExceptionLog)
                    {
                        CachedExceptionLogs[(int)numberKey].RequestsCount++;
                        return genericResultExceptionLog;
                    }
                    return default;
                default:
                    throw new ArgumentException();
            }
        }
        public static void SetValue<CachedValueT>(CachedValueT input, string? stringKey = null)
        {
            if (!_isCacheInitialized)
            { InitializeCache(); }

            _semaphore.Wait();
            try
            {
                SetValueInternal<CachedValueT>(input, stringKey);
            }
            catch
            {
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
            CheckMemory();
        }
        private static void SetValueInternal<CachedValueT>(CachedValueT input, string? stringKey = null)
        {
            switch (typeof(CachedValueT).Name)
            {
                case nameof(TreeExtended):
                    if (stringKey == null)
                        throw new ArgumentNullException(nameof(stringKey), "Key is required to add Tree to the cache.");
                    if (input is TreeExtended genericResultTreeExtended)
                    {
                        if (!CachedTrees.ContainsKey(stringKey))
                            CachedTrees[stringKey] = new CachedItem<TreeExtended>();

                        CachedTrees[stringKey].Item = genericResultTreeExtended;
                    }
                    break;
                case nameof(EventLogs):
                    if (stringKey == null)
                        throw new ArgumentNullException(nameof(stringKey), "Key is required to add EventLogs to the cache.");
                    if (input is EventLogs genericResultEventLogs)
                        CachedEventLogsSearch[stringKey] = genericResultEventLogs;
                    break;
                default:
                    throw new ArgumentException();
            }
        }
        public static void SetValue<CachedValueT>(CachedValueT input, int? numberKey = null)
        {
            if (!_isCacheInitialized)
            { InitializeCache(); }

            _semaphore.Wait();
            try
            {
                SetValueInternal<CachedValueT>(input, numberKey);
            }
            catch
            {
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
            CheckMemory();
        }
        private static void SetValueInternal<CachedValueT>(CachedValueT input, int? numberKey = null)
        {
            switch (typeof(CachedValueT).Name)
            {
                case nameof(ExceptionLog):
                    if (numberKey == null)
                        throw new ArgumentNullException(nameof(numberKey), "Key is required to add ExceptionLog to the cache.");
                    if (input is ExceptionLog genericResultExceptionLog)
                        CachedExceptionLogs[(int)numberKey] = new ExceptionLogExtended(genericResultExceptionLog);
                    break;
                default:
                    throw new ArgumentException();
            }
        }
        public static void ForceValidation<CachedValueT>(string? stringKey = null)
        {
            if (!_isCacheInitialized)
            { InitializeCache(); }

            _semaphore.Wait();
            try
            {
                ForceValidationInternal<CachedValueT>(stringKey);
            }
            catch
            {
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        private static void ForceValidationInternal<CachedValueT>(string? stringKey = null)
        {
            switch (typeof(CachedValueT).Name)
            {
                case nameof(TreeExtended):
                    if (stringKey == null)
                        throw new ArgumentNullException(nameof(stringKey), "Key is required to force validation of the Tree in the cache.");
                    if (CachedTrees.ContainsKey(stringKey))
                        CachedTrees[stringKey].ForceValidation();
                    break;
                case nameof(EventLogs):
                    if (stringKey == null)
                        throw new ArgumentNullException(nameof(stringKey), "Key is required to force validation of the EventLogs in the cache.");
                    if (CachedEventLogsSearch.ContainsKey(stringKey))
                        CachedEventLogsSearch.Remove(stringKey);
                    break;
                default:
                    throw new ArgumentException();
            }
        }
        public static void ForceValidation<CachedValueT>(int? numberKey = null)
        {
            if (!_isCacheInitialized)
            { InitializeCache(); }

            _semaphore.Wait();
            try
            {
                ForceValidationInternal<CachedValueT>(numberKey);
            }
            catch
            {
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        private static void ForceValidationInternal<CachedValueT>(int? numberKey = null)
        {
            switch (typeof(CachedValueT).Name)
            {
                case nameof(ExceptionLog):
                    if (numberKey == null)
                        throw new ArgumentNullException(nameof(numberKey), "Key is required to force validation of the ExceptionLog in the cache.");
                    if (CachedExceptionLogs.ContainsKey((int)numberKey))
                        CachedExceptionLogs.Remove((int)numberKey);
                    break;
                default:
                    throw new ArgumentException();
            }
        }
    }

    public class CachedItem<T> : ICacheableValue
    {
        public int RequestsCount { get; set; }

        private bool _isValid;
        private DateTime _updatedOn;
        private T? _item;

        public T? Item { get { return _isValid ? _updatedOn.AddMinutes(CacheManager.MINUTES_TO_UPDATE) > DateTime.Now ? _item : default : default; } set { _item = value; _isValid = true; _updatedOn = DateTime.Now; } }

        public CachedItem() 
        {
            _isValid = false;
            RequestsCount = 0;
        }

        public void ForceValidation()
        {
            _isValid = false;
        }
    }

}
