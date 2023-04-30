using API_TestProject.Core.Model;
using API_TestProject.DataBase;
using API_TestProject.DataBase.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_TestProject.Core
{
    public class TreeService
    {
        private readonly ILogger<TreeService> _logger;
        private readonly APIContext _context;

        public TreeService(APIContext context, ILogger<TreeService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Method retrieves a Tree object associated with the given name from either a cache or a database. If the tree is not found in the cache, it looks for it in the database. 
        /// If it's still not found, a new tree object is created and saved to the database. Finally, the method returns the retrieved or created Tree object.
        /// </summary>
        internal async Task<TreeExtended> GetTreeAsync(string treeName)
        {
            var tree = CacheManager.GetValue<TreeExtended>(stringKey: treeName);

            if (tree != null)
            { return tree; }

            _logger.LogInformation($"Tree with name {treeName} was not found in the cache. Starting request to the DataBase.");
            var treeDB = await _context.Trees.Include(t => t.Nodes.Where(n => n.ParentNodeId == null)).FirstOrDefaultAsync(x => x.Name.Equals(treeName));
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
            var nodes = _context.Nodes.Where(n => n.TreeId == treeDB.TreeId).ToList();
            tree = new TreeExtended(treeDB, nodes);
            CacheManager.SetValue<TreeExtended>(tree, stringKey: treeName);

            return tree;
        }

        /// <summary>
        /// This method creates a new node in a tree with a specified name and parent node ID. It then creates a new node object and adds it to the appropriate collection of nodes. 
        /// Finally, it saves the changes to the database and returns a 200 status code.
        /// </summary>
        internal async Task<ActionResult> CreateNode(string treeName, int parentNodeId, string nodeName)
        {
            var tree = await GetTreeInternal(treeName);

            var IsTreeSibling = parentNodeId == -1;
            var node = new Node() { Name = nodeName, TreeId = tree.TreeId, ParentNodeId = IsTreeSibling ? null : parentNodeId };

            if (IsTreeSibling)
            {
                CheckIsNameUnique(tree.Nodes, node);
                tree.Nodes.Add(node);
            }
            else if (tree.AllNodesMap.ContainsKey(parentNodeId))
            {
                var parentNode = await _context.Nodes.Include(t => t.ChildrenNodes).FirstOrDefaultAsync(x => x.NodeId == parentNodeId);
 
                if (parentNode == null)
                {
                    CacheManager.ForceValidation<TreeExtended>(stringKey: treeName);
                    throw new SecureException($"Node with id:{parentNodeId} doesn't exist in {treeName} Tree.");
                }
                if (parentNode.TreeId != node.TreeId)
                {
                    throw new SecureException($"All children nodes must belong to the same tree as their parent node.");
                }
                CheckIsNameUnique(parentNode.ChildrenNodes, node);

                parentNode.ChildrenNodes.Add(node);
                tree.AllNodesMap[parentNode.NodeId].ChildrenNodes = parentNode.ChildrenNodes;
            }
            else
            {
                throw new SecureException($"Node with id:{parentNodeId} doesn't exist in {treeName} Tree.");
            }

            _context.Nodes.Add(node);
            await _context.SaveChangesAsync();
            tree.AllNodesMap[node.NodeId] = node;

            return new ContentResult
            {
                ContentType = "application/json",
                StatusCode = 200
            };
        }

        /// <summary>
        /// This method deletes a node from a tree by its ID. It first checks if the node exists in the specified tree, and then checks if the node has any children nodes. 
        /// If the node has children nodes, it throws an exception. If the node doesn't have any children nodes, it removes the node from the tree and the parent node's list of children nodes (if it has a parent node). 
        /// Finally, it removes the node from the database and returns a JSON content result with a 200 status code.
        /// </summary>
        internal async Task<ActionResult> DeleteNode(string treeName, int nodeId)
        {
            var tree = await GetTreeInternal(treeName);
            var node = await GetNodeInternal(tree, nodeId);
            if (node.ChildrenNodes.Count > 0)
            {
                throw new SecureException($"It is not possible to delete node which has children nodes. Delete children nodes first.");
                // The easiest implementation due to lack of time
                // It is possible: 1) move children nodes to another parent node 2) delete all children nodes automatically not manually
            }

            if (node.ParentNodeId != null)
            {
                var parentNode = await _context.Nodes.Include(t => t.ChildrenNodes).FirstOrDefaultAsync(x => x.NodeId == node.ParentNodeId);
                if (parentNode == null)
                { throw new SecureException($"Parent node with id: {node.ParentNodeId} doesn`t exist. Contact system administrator."); }
                if (parentNode.TreeId != tree.TreeId)
                { throw new SecureException($"Parent node with id: {node.ParentNodeId} doesn`t exist in {treeName} tree. Contact system administrator."); }

                parentNode.ChildrenNodes.Remove(node);
                tree.AllNodesMap[parentNode.NodeId].ChildrenNodes = parentNode.ChildrenNodes;
            }

            tree.Nodes.Remove(node);
            tree.AllNodesMap.Remove(node.NodeId);

            _context.Nodes.Remove(node);
            await _context.SaveChangesAsync();

            return new ContentResult
            {
                ContentType = "application/json",
                StatusCode = 200
            };
        }

        /// <summary>
        /// The method retrieves the tree and node. It then updates the node's name with the new name specified in the method parameter. 
        /// It checks whether the node is a direct child of the tree or a child of another node within the tree, and ensures that the new name is unique within the corresponding list of nodes.
        /// It saves the changes to the database and returns a success response with a status code of 200.
        /// </summary>
        public async Task<ActionResult> RenameNode(string treeName, int nodeId, string newNodeName)
        {
            var tree = await GetTreeInternal(treeName);
            var node = await GetNodeInternal(tree, nodeId);

            node.Name = newNodeName;

            var IsTreeSibling = node.ParentNodeId == null;

            if (IsTreeSibling)
            {
                CheckIsNameUnique(tree.Nodes, node);
            }
            else if (tree.AllNodesMap.ContainsKey((int)node.ParentNodeId))
            {
                var parentNode = await _context.Nodes.Include(t => t.ChildrenNodes).FirstOrDefaultAsync(x => x.NodeId == node.ParentNodeId);
                CheckIsNameUnique(parentNode.ChildrenNodes, node);

                if (parentNode.TreeId != node.TreeId)
                {
                    throw new SecureException($"All children nodes must belong to the same tree as their parent node.");
                }
            }
            else
            {
                throw new SecureException($"Node with id:{node.ParentNodeId} doesn't exist in {treeName} Tree.");
            }

            await _context.SaveChangesAsync();

            return new ContentResult
            {
                ContentType = "application/json",
                StatusCode = 200
            };
        }



        private void CheckIsNameUnique(ICollection<Node> nodes, Node targetNode)
        {
            if (nodes == null)
            { throw new ArgumentNullException(nameof(nodes)); }
            if (targetNode == null)
            { throw new ArgumentNullException(nameof(targetNode)); }
            
            if (nodes.Any(x => x.Name.Equals(targetNode.Name)))
            {
                var TreeOrNodeChild = targetNode.ParentNodeId == null ? "Tree" : $"node with id {targetNode.ParentNodeId}";
                throw new SecureException($"Node with name: {targetNode.Name} can not be added as a child of {TreeOrNodeChild}. The node name must be unique across all siblings.");
            }
        }

        private async Task<TreeExtended> GetTreeInternal(string treeName)
        {
            var tree = CacheManager.GetValue<TreeExtended>(stringKey: treeName);

            if (tree == null)
            {
                _logger.LogInformation($"Tree with name {treeName} was not found in the cache. Starting request to the DataBase.");
                var treeDB = await _context.Trees.Include(t => t.Nodes.Where(n => n.ParentNodeId == null)).ThenInclude(n => n.ChildrenNodes).FirstOrDefaultAsync(x => x.Name.Equals(treeName));
                if (treeDB == null)
                {
                    throw new SecureException($"Tree with name {treeName} doesn't exist.");
                }
                var nodes = _context.Nodes.Where(n => n.TreeId == treeDB.TreeId).ToList();
                tree = new TreeExtended(treeDB, nodes);
                CacheManager.SetValue<TreeExtended>(tree, stringKey: treeName);
            }
            return tree;
        }

        private async Task<Node> GetNodeInternal(TreeExtended tree, int nodeId)
        {
            if (!tree.AllNodesMap.ContainsKey(nodeId))
            { throw new SecureException($"Node with id: {nodeId} doesn`t exist in {tree.Name} tree."); }

            var node = await _context.Nodes.Include(t => t.ChildrenNodes).FirstOrDefaultAsync(x => x.NodeId == nodeId);
            if (node == null)
            { throw new SecureException($"Node with id: {nodeId} doesn`t exist."); }
            if (node.TreeId != tree.TreeId)
            { throw new SecureException($"Node with id: {nodeId} doesn`t exist in {tree.Name} tree."); }

            return node;
        }
    }
}
