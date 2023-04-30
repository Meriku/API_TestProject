using API_TestProject.Core.Model;
using API_TestProject.DataBase.Model;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace API_TestProject.Core
{
    public static class CacheManager
    {
        public static readonly int MINUTES_TO_UPDATE = 5;
        public static readonly int MAX_EVENT_LOG_ITEMS_COUNT = 1000;


        private static bool _isCacheInitialized = false;

        private static Dictionary<string, CachedItem<TreeExtended>> CachedTrees;
        private static Dictionary<int, ExceptionLog> CachedExceptionLogs;
        private static Dictionary<string, EventLogs> CachedEventLogs;

        private static void InitializeCache()
        {
            CachedTrees = new Dictionary<string, CachedItem<TreeExtended>>();
            CachedExceptionLogs = new Dictionary<int, ExceptionLog>();
            CachedEventLogs = new Dictionary<string, EventLogs>();

            _isCacheInitialized = true;
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
                    if (CachedEventLogs.Keys.ToHashSet() is HashSetT genericResultEventLogs)
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

            switch (typeof(CachedValueT).Name)
            {
                case nameof(TreeExtended):
                    if (stringKey == null)
                        throw new ArgumentNullException(nameof(stringKey), "Key is required to retrieve Tree from the cache.");
                    if (CachedTrees.ContainsKey(stringKey) && CachedTrees[stringKey].Item is CachedValueT genericResultTreeExtended)
                        return genericResultTreeExtended;
                    return default;
                case nameof(EventLogs):
                    if (stringKey == null)
                        throw new ArgumentNullException(nameof(stringKey), "Key is required to retrieve EventLogs from the cache.");
                    if (CachedEventLogs.ContainsKey(stringKey) && CachedEventLogs[stringKey] is CachedValueT genericResultEventLogs)
                        return genericResultEventLogs;
                    return default;
                default:
                    throw new ArgumentException();
            }    
        }
        public static CachedValueT? GetValue<CachedValueT>(int? numberKey = null)
        {
            if (!_isCacheInitialized)
            { InitializeCache(); }

            switch (typeof(CachedValueT).Name)
            {
                case nameof(ExceptionLog):
                    if (numberKey == null)
                        throw new ArgumentNullException(nameof(numberKey), "Key is required to retrieve ExceptionLog from the cache.");
                    if (CachedExceptionLogs.ContainsKey((int)numberKey) && CachedExceptionLogs[(int)numberKey] is CachedValueT genericResultExceptionLog)
                        return genericResultExceptionLog;
                    return default;
                default:
                    throw new ArgumentException();
            }
        }
        public static void SetValue<CachedValueT>(CachedValueT input, string? stringKey = null)
        {
            if (!_isCacheInitialized)
            { InitializeCache(); }

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
                        CachedEventLogs[stringKey] = genericResultEventLogs;
                    break;
                default:
                    throw new ArgumentException();
            }
        }
        public static void SetValue<CachedValueT>(CachedValueT input, int? numberKey = null)
        {
            if (!_isCacheInitialized)
            { InitializeCache(); }

            switch (typeof(CachedValueT).Name)
            {
                case nameof(ExceptionLog):
                    if (numberKey == null)
                        throw new ArgumentNullException(nameof(numberKey), "Key is required to add ExceptionLog to the cache.");
                    if (input is ExceptionLog genericResultExceptionLog)
                        CachedExceptionLogs[(int)numberKey] = genericResultExceptionLog;
                    break;
                default:
                    throw new ArgumentException();
            }
        }
        public static void ForceValidation<CachedValueT>(string? stringKey = null)
        {
            if (!_isCacheInitialized)
            { InitializeCache(); }

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
                    if (CachedEventLogs.ContainsKey(stringKey))
                        CachedEventLogs.Remove(stringKey);
                    break;
                default:
                    throw new ArgumentException();
            }
        }
        public static void ForceValidation<CachedValueT>(int? numberKey = null)
        {
            if (!_isCacheInitialized)
            { InitializeCache(); }

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

    public class CachedItem<T>
    {
        private bool _isValid;
        private DateTime _updatedOn;
        private T? _item;

        public T? Item { get { return _isValid ? _updatedOn.AddMinutes(CacheManager.MINUTES_TO_UPDATE) > DateTime.Now ? _item : default : default; } set { _item = value; _isValid = true; _updatedOn = DateTime.Now; } }

        public CachedItem() 
        {
            _isValid = false;
        }

        public void ForceValidation()
        {
            _isValid = false;
        }
    }

}
