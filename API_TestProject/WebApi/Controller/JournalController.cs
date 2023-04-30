﻿using API_TestProject.Core;
using API_TestProject.Core.Model;
using API_TestProject.DataBase;
using API_TestProject.DataBase.Model;
using API_TestProject.WebApi.Model.Request;
using API_TestProject.WebApi.Model.Response;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API_TestProject.WebApi.Controller
{
    [Route("Api/[controller]")]
    [ApiController]
    public class JournalController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly JournalService _journalService;

        public JournalController(IMapper mapper, JournalService journalService)
        {
            _mapper = mapper;
            _journalService = journalService;
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
            var result = await _journalService.GetSingle(id);
            return _mapper.Map<ExceptionLog, EventLogItemDTO>(result);
        }
    }
}
