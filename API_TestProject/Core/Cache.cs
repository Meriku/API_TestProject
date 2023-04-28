using API_TestProject.Core.Model;
using API_TestProject.DataBase.Model;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace API_TestProject.Core
{
    public static class CacheManager
    {
        private static CachedItem<TreeExtended> _tree;
        public static TreeExtended CachedTree { get { if (_tree == null) { _tree = new CachedItem<TreeExtended>(); }; return _tree.Item; } set { _tree.Item = value; } }


        // TODO: refactor this

    }

    public class CachedItem<T>
    {
        private const int MINUTES_TO_UPDATE = 5;

        private bool _isValid;
        private DateTime _updatedOn;
        private T _item;

        public T? Item { get { return _isValid ? _updatedOn.AddMinutes(MINUTES_TO_UPDATE) > DateTime.Now ? _item : default(T) : default(T); } set { _item = value; _isValid = true; _updatedOn = DateTime.Now; } }

        public CachedItem() 
        {
            _isValid = false;
        }
    }

}
