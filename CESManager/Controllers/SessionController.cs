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
                if (response.Data == null)
                {
                    return NotFound(response.StatusCode);
                }
            }
            catch
            {
                StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(response); 
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSingle(int id)
        {
            ServiceResponse<GetSessionDto> response = await _sessionService.GetSessionById(id);
            try
            {
                if (response.Data == null)
                {
                    return NotFound(response.StatusCode);
                }
            }
            catch
            {
                StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> AddSession(AddSessionDto newSession)
        {
            ServiceResponse<List<GetSessionDto>> response = await _sessionService.AddSession(newSession);
            try
            {
                if (response.Data == null)
                {
                    return BadRequest(response.StatusCode);
                }
            }
            catch
            {
                StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSession (UpdateSessionDto updatedSession)
        {
            ServiceResponse<GetSessionDto> response = await _sessionService.UpdateSession(updatedSession);
            try
            {
                if (response.StatusCode == CESManagerStatusCode.SessionNotFound)
                {
                    return NotFound(response.StatusCode);
                }
                if (response.StatusCode == CESManagerStatusCode.NegativeDuration)
                {
                    return BadRequest(response.StatusCode);
                }
                if (response.StatusCode == CESManagerStatusCode.InternalServerError)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            catch 
            {
                StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            ServiceResponse<List<GetSessionDto>> response = await _sessionService.DeleteSession(id);
            try
            {
                if (response.Data == null)
                {
                    return NotFound(response.StatusCode);
                }
            }
            catch
            {
                StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(response);
        }
    }
}