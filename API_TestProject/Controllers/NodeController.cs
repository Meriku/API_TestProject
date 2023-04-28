using API_TestProject.Data;
using API_TestProject.Model;
using Microsoft.AspNetCore.Mvc;

namespace API_TestProject.Controllers
{
    [Route("Api/Tree/[controller]")]
    [ApiController]
    public class NodeController : ControllerBase
    {
        private readonly APIContext _context;
        public NodeController(APIContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Create a new node in your tree. 
        /// You must to specify a parent node ID that belongs to your tree. 
        /// A new node name must be unique across all siblings.
        /// </summary>
        [HttpPost("Create")]
        public async Task<ActionResult> CreateNode([FromQuery] string treeName, [FromQuery] int parentNodeId, [FromQuery] string nodeName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete an existing node in your tree. 
        /// You must specify a node ID that belongs your tree.
        /// </summary>
        [HttpPost("Delete")]
        public async Task<ActionResult> DeleteNode([FromQuery] string treeName, [FromQuery] int nodeId)
        {   
            throw new NotImplementedException();
        }

        /// <summary>
        /// Rename an existing node in your tree. 
        /// You must specify a node ID that belongs your tree. 
        /// A new name of the node must be unique across all siblings.
        /// </summary>
        [HttpPost("Rename")]
        public async Task<ActionResult> RenameNode([FromQuery] string treeName, [FromQuery] int nodeId, [FromQuery] string newNodeName)
        {
            throw new NotImplementedException();
        }
    }
}
