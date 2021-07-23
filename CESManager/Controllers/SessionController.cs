using System;
using CESManager.Dtos.Session;
using CESManager.Models;
using CESManager.Services.SessionService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

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
            ServiceResponse<List<GetSessionDto>> response = await _sessionService.GetAllSessions();
            try
            {
                if (response.StatusCode == CESManagerStatusCode.SessionNotFound)
                {
                    return NotFound(response.Message);
                }
            }
            catch
            {
                StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(response.Data); 
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSingle(int id)
        {
            ServiceResponse<GetSessionDto> response = await _sessionService.GetSessionById(id);
            try
            {
                if (response.StatusCode == CESManagerStatusCode.SessionNotFound)
                {
                    return NotFound(response.Message);
                }
            }
            catch
            {
                StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(response.Data);
        }

        [HttpPost]
        public async Task<IActionResult> AddSession(AddSessionDto newSession)
        {
            ServiceResponse<List<GetSessionDto>> response = await _sessionService.AddSession(newSession);
            try
            {
                if (response.StatusCode == CESManagerStatusCode.NegativeDuration)
                {
                    return BadRequest(response.Message);
                }
            }
            catch
            {
                StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(response.Data);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSession (UpdateSessionDto updatedSession)
        {
            ServiceResponse<GetSessionDto> response = await _sessionService.UpdateSession(updatedSession);
            try
            {
                if (response.StatusCode == CESManagerStatusCode.SessionNotFound)
                {
                    return NotFound(response.Message);
                }
                if (response.StatusCode == CESManagerStatusCode.NegativeDuration)
                {
                    return BadRequest(response.Message);
                }
            }
            catch 
            {
                StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(response.Data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            ServiceResponse<List<GetSessionDto>> response = await _sessionService.DeleteSession(id);
            try
            {
                if (response.StatusCode == CESManagerStatusCode.SessionNotFound)
                {
                    return NotFound(response.Message);
                }
            }
            catch
            {
                StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(response.Data);
        }
    }
}