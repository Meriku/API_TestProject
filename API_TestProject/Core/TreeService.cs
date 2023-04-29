using API_TestProject.Core.Model;
using API_TestProject.DataBase;
using API_TestProject.DataBase.Model;
using API_TestProject.WebApi.Controller;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace API_TestProject.Core
{
    public class TreeService
    {
        private readonly ILogger<TreeService> _logger;
        private readonly APIContext _context;
        private readonly IMapper _mapper;
        public TreeService(APIContext context, ILogger<TreeService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Method retrieves a Tree object associated with the given name from either a cache or a database. If the tree is not found in the cache, it looks for it in the database. 
        /// If it's still not found, a new tree object is created and saved to the database. Finally, the method returns the retrieved or created Tree object.
        /// </summary>
        internal async Task<Tree> GetTreeAsync(string treeName)
        {
            var tree = CacheManager.GetValue<TreeExtended>(key: treeName);

            if (tree != null)
            { return tree; }

            _logger.LogInformation($"Tree with name {treeName} was not found in the cache. Starting request to the DataBase.");
            var treeDB = await _context.Trees.FirstOrDefaultAsync(x => x.Name.Equals(treeName));
            if (treeDB == null)
            {
                _logger.LogInformation($"Tree with name {treeName} was not found in the DataBase. Initializing new tree.");
                treeDB = new Tree()
                {
                    Name = treeName,
                    Nodes = new List<Node>()
                };
                _context.Trees.Add(treeDB);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Tree with name {treeName} was successfully saved to the DataBase.");
            }
            tree = new TreeExtended(treeDB);
            CacheManager.SetValue<TreeExtended>(tree, key: treeName);

            return tree;
        }

        internal async Task<ActionResult> CreateNode(string treeName, int parentNodeId, string nodeName)
        {
            var tree = await _context.Trees.FirstOrDefaultAsync(x => x.Name.Equals(treeName));
            var node = new Node() { Name = nodeName };

            throw new NotImplementedException();
        }




    }
}
