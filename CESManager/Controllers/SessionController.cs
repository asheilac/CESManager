using System;
using CESManager.Dtos.Session;
using CESManager.Models;
using CESManager.Services.SessionService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CESManager.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class SessionController : ControllerBase
    {
        private readonly ISessionService _sessionService;

        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> Get()
        {
            ServiceResponse<List<GetSessionDto>> response;
            try
            {
                response = await _sessionService.GetAllSessions();
                if (response.Data == null)
                {
                    return BadRequest("There was a problem");
                }
            }
            catch 
            {
                return BadRequest("There was a problem");
            }
            return Ok(response); 
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSingle(int id)
        {
            ServiceResponse<GetSessionDto> response = await _sessionService.GetSessionById(id);
            if (response.Data == null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> AddSession(AddSessionDto newSession)
        {
            ServiceResponse<List<GetSessionDto>> response = await _sessionService.AddSession(newSession);
            if (response.Data == null)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSession (UpdateSessionDto updatedSession)
        {
            ServiceResponse<GetSessionDto> response  = await _sessionService.UpdateSession(updatedSession);
            if(response.Data == null)
            { 
                return NotFound(response);
            }
            if (response.Data.Duration <= 0)
            {
                return BadRequest("EndDateTime must be later than StartDateTime.");
            }
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            ServiceResponse<List<GetSessionDto>> response = await _sessionService.DeleteSession(id);
            if (response.Data == null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}