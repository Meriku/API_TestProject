using API_TestProject.DataBase;
using API_TestProject.WebApi.Model.Request;
using API_TestProject.WebApi.Model.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_TestProject.WebApi.Controller
{
    [Route("Api/[controller]")]
    [ApiController]
    public class JournalController : ControllerBase
    {
        private readonly APIContext _context;
        public JournalController(APIContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Provides the pagination API. 
        /// Skip means the number of items should be skipped by server. 
        /// Take means the maximum number items should be returned by server. 
        /// All fields of the filter are optional.
        /// </summary>
        [HttpPost("GetRange")]
        public async Task<ActionResult<EventLogListDTO>> GetRange([FromQuery] int skip, [FromQuery] int take, [FromBody] FilterDTO filter)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the information about an particular event by ID.
        /// </summary>
        [HttpPost("GetSingle")]
        public async Task<ActionResult<EventLogItemDTO>> GetSingle([FromQuery] int id)
        {
            throw new NotImplementedException();
        }
    }
}
