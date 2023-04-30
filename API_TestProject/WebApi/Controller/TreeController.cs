using API_TestProject.Core;
using API_TestProject.Core.Model;
using API_TestProject.DataBase.Model;
using API_TestProject.WebApi.Model.Response;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API_TestProject.WebApi.Controller
{
    [Route("Api/[controller]")]
    [ApiController]
    public class TreeController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly TreeService _treeService;

        public TreeController(IMapper mapper, TreeService treeService)
        {
            _mapper = mapper;
            _treeService = treeService;
        }

        /// <summary>
        /// Returns your entire tree. 
        /// If your tree doesn't exist it will be created automatically.
        /// </summary>
        [HttpPost("Get")]
        public async Task<ActionResult<TreeDTO>> GetTree([FromQuery] string treeName)
        {
            var tree = await _treeService.GetTreeAsync(treeName);
            return _mapper.Map<TreeExtended, TreeDTO>(tree);
        }
    }
}
