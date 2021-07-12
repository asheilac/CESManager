using System.Collections.Generic;
using System.Threading.Tasks;
using CESManager.Dtos.Session;
using CESManager.Models;

namespace CESManager.Services.SessionService
{
    public interface ISessionService
    {
         Task<ServiceResponse<List<GetSessionDto>>> GetAllSessions();
         Task<ServiceResponse<GetSessionDto>> GetSessionById (int id);
         Task<ServiceResponse<List<GetSessionDto>>> AddSession (AddSessionDto newSession);
         Task<ServiceResponse<List<GetSessionDto>>> DeleteSession (int id);
         Task<ServiceResponse<GetSessionDto>> UpdateSession (UpdateSessionDto updatedSession);
    }
}