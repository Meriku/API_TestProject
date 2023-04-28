using API_TestProject.Data;
using API_TestProject.Model;
using API_TestProject.Model.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_TestProject.Controllers
{
    [Route("Api/[controller]")]
    [ApiController]
    public class TreeController : ControllerBase
    {
        private readonly APIContext _context;
        public TreeController(APIContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns your entire tree. 
        /// If your tree doesn't exist it will be created automatically.
        /// </summary>
        [HttpPost("Get")]
        public async Task<ActionResult<TreeDTO>> GetTree([FromQuery] string treeName)
        {
            var tree = await _context.Trees.FirstOrDefaultAsync(x => x.Name.Equals(treeName));

            if (tree == null)
            {
                throw new Exception("Not Found");
            }

            throw new NotImplementedException();
            //return Mapper.Map<Tree, TreeDTO>(tree);
        }
    }
}
