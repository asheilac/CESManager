using System.Threading.Tasks;
using CESManager.Data;
using CESManager.Dtos.User;
using CESManager.Models;
using Microsoft.AspNetCore.Mvc;

namespace CESManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;
        public AuthController(IAuthRepository authRepo)
        {
            _authRepo = authRepo;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserRegisterDto request)
        {
            ServiceResponse<int> response = await _authRepo.Register(
                new User {Username = request.Username}, request.Password
            );
            if(response.StatusCode == CESManagerStatusCode.InvalidRegister)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginDto request)
        {
            ServiceResponse<string> response = await _authRepo.Login(
                request.Username, request.Password
            );
            if(response.StatusCode == CESManagerStatusCode.InvalidRegister)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}