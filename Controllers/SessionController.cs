using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CESManager.Dtos.Session;
using CESManager.Models;
using CESManager.Services.SessionService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            return Ok(await _sessionService.GetAllSessions());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSingle(int id)
        {
            return Ok(await _sessionService.GetSessionById(id));
        }

        [HttpPost]
        public async Task<IActionResult> AddSession(AddSessionDto newSession)
        {
            return Ok(await _sessionService.AddSession(newSession));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSession (UpdateSessionDto updatedSession)
        {
            ServiceResponse<GetSessionDto> response  = await _sessionService.UpdateSession(updatedSession);
            if(response.Data != null)
            {
                return Ok(response);
            }
            return NotFound(response);
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