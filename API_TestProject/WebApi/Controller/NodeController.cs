using API_TestProject.Core;
using Microsoft.AspNetCore.Mvc;

namespace API_TestProject.WebApi.Controller
{
    [Route("Api/Tree/[controller]")]
    [ApiController]
    public class NodeController : ControllerBase
    {
        private readonly TreeService _treeService;

        public NodeController(TreeService treeService)
        {
            _treeService = treeService;
        }

        /// <summary>
        /// Create a new node in your tree. 
        /// You must to specify a parent node ID that belongs to your tree. 
        /// A new node name must be unique across all siblings.
        /// Create a direct child of the tree with parent node ID = -1.
        /// </summary>
        [HttpPost("Create")]
        public async Task<ActionResult> CreateNode([FromQuery] string treeName, [FromQuery] int parentNodeId, [FromQuery] string nodeName)
        {
            var result = await _treeService.CreateNode(treeName, parentNodeId, nodeName);
            return result;
        }

        /// <summary>
        /// Delete an existing node in your tree. 
        /// You must specify a node ID that belongs your tree.
        /// </summary>
        [HttpPost("Delete")]
        public async Task<ActionResult> DeleteNode([FromQuery] string treeName, [FromQuery] int nodeId)
        {
            var result = await _treeService.DeleteNode(treeName, nodeId);
            return result;
        }

        /// <summary>
        /// Rename an existing node in your tree. 
        /// You must specify a node ID that belongs your tree. 
        /// A new name of the node must be unique across all siblings.
        /// </summary>
        [HttpPost("Rename")]
        public async Task<ActionResult> RenameNode([FromQuery] string treeName, [FromQuery] int nodeId, [FromQuery] string newNodeName)
        {
            var result = await _treeService.RenameNode(treeName, nodeId, newNodeName);
            return result;
        }
    }
}
