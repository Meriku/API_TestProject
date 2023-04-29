using API_TestProject.Core.Model;
using API_TestProject.DataBase.Model;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace API_TestProject.Core
{
    public static class CacheManager
    {
        private static bool _isCacheInitialized = false;

        private static Dictionary<string, CachedItem<TreeExtended>> CachedTrees;

        public static TInput? GetValue<TInput>(string? key = null)
        {
            if (!_isCacheInitialized)
            { InitializeCache(); }

            switch (typeof(TInput).Name)
            {
                case nameof(TreeExtended):
                    if (key == null)
                        throw new ArgumentNullException(nameof(key), "Key is required to retrieve Tree from the cache");
                    if (CachedTrees.ContainsKey(key) && CachedTrees[key].Item is TInput genericResult)
                        return genericResult;
                    return default;
                default:
                    throw new NotImplementedException();
            }
            
        }

        public static void SetValue<TInput>(TInput input, string? key = null)
        {
            switch (typeof(TInput).Name)
            {
                case nameof(TreeExtended):
                    if (key == null)
                        throw new ArgumentNullException(nameof(key), "Key is required to add Tree to the cache");
                    if (input is TreeExtended genericResult)
                    {
                        if (!CachedTrees.ContainsKey(key))
                            CachedTrees[key] = new CachedItem<TreeExtended>();

                        CachedTrees[key].Item = genericResult;
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private static void InitializeCache()
        {
            CachedTrees = new Dictionary<string, CachedItem<TreeExtended>>();

            _isCacheInitialized = true;
        }

    }

    public class CachedItem<T>
    {
        private const int MINUTES_TO_UPDATE = 5;

        private bool _isValid;
        private DateTime _updatedOn;
        private T? _item;

        public T? Item { get { return _isValid ? _updatedOn.AddMinutes(MINUTES_TO_UPDATE) > DateTime.Now ? _item : default : default; } set { _item = value; _isValid = true; _updatedOn = DateTime.Now; } }

        public CachedItem() 
        {
            _isValid = false;
        }
    }

}
