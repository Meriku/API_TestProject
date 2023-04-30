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

        private static void InitializeCache()
        {
            CachedTrees = new Dictionary<string, CachedItem<TreeExtended>>();
            CachedExceptionLogs = new Dictionary<int, ExceptionLog>();

            _isCacheInitialized = true;

        }
        public static TInput? GetValue<TInput>(string? stringKey = null)
        {
            if (!_isCacheInitialized)
            { InitializeCache(); }

            switch (typeof(TInput).Name)
            {
                case nameof(TreeExtended):
                    if (stringKey == null)
                        throw new ArgumentNullException(nameof(stringKey), "Key is required to retrieve Tree from the cache.");
                    if (CachedTrees.ContainsKey(stringKey) && CachedTrees[stringKey].Item is TInput genericResultTreeExtended)
                        return genericResultTreeExtended;
                    return default;
                default:
                    throw new ArgumentException();
            }    
        }
        public static TInput? GetValue<TInput>(int? numberKey = null)
        {
            if (!_isCacheInitialized)
            { InitializeCache(); }

            switch (typeof(TInput).Name)
            {
                case nameof(ExceptionLog):
                    if (numberKey == null)
                        throw new ArgumentNullException(nameof(numberKey), "Key is required to retrieve ExceptionLog from the cache.");
                    if (CachedExceptionLogs.ContainsKey((int)numberKey) && CachedExceptionLogs[(int)numberKey] is TInput genericResultExceptionLog)
                        return genericResultExceptionLog;
                    return default;
                default:
                    throw new ArgumentException();
            }
        }
        public static void SetValue<TInput>(TInput input, string? stringKey = null)
        {
            if (!_isCacheInitialized)
            { InitializeCache(); }

            switch (typeof(TInput).Name)
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
                default:
                    throw new ArgumentException();
            }
        }
        public static void SetValue<TInput>(TInput input, int? numberKey = null)
        {
            if (!_isCacheInitialized)
            { InitializeCache(); }

            switch (typeof(TInput).Name)
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
        public static void ForceValidation<TInput>(string? stringKey = null)
        {
            if (!_isCacheInitialized)
            { InitializeCache(); }

            switch (typeof(TInput).Name)
            {
                case nameof(TreeExtended):
                    if (stringKey == null)
                        throw new ArgumentNullException(nameof(stringKey), "Key is required to force validation of the Tree in the cache.");
                    if (CachedTrees.ContainsKey(stringKey))
                        CachedTrees[stringKey].ForceValidation();
                    break;
                default:
                    throw new ArgumentException();
            }
        }
        public static void ForceValidation<TInput>(int? numberKey = null)
        {
            if (!_isCacheInitialized)
            { InitializeCache(); }

            switch (typeof(TInput).Name)
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
