using API_TestProject.Core.Model;
using API_TestProject.DataBase;
using API_TestProject.DataBase.Model;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API_TestProject.Core
{
    public class TreeService
    {
        private readonly APIContext _context;
        private readonly IMapper _mapper;
        public TreeService(APIContext context)
        {
            _context = context;
        }

        internal async Task<Tree> GetTreeAsync(string treeName)
        {
            var tree = CacheManager.CachedTree;

            if (tree == null)
            {
                var treeDB = await _context.Trees.FirstOrDefaultAsync(x => x.Name.Equals(treeName));
                tree = new TreeExtended(treeDB);
                if (tree == null)
                {
                    throw new Exception("Not Found");
                }
                CacheManager.CachedTree = tree;
            }

            return tree;
        }




    }
}
