using System.Collections.Generic;
using System.Threading.Tasks;
using CESManager.Dtos.Session;
using CESManager.Models;

namespace CESManager.Services.SessionService
{
    public interface ISessionService
    {
         Task<ServiceResponse<List<GetSessionDto>>> GetAllSessions(int userId);
         Task<ServiceResponse<GetSessionDto>> GetSessionById (int id, int userId);
         Task<ServiceResponse<List<GetSessionDto>>> AddSession (AddSessionDto newSession);
         Task<ServiceResponse<List<GetSessionDto>>> DeleteSession (int id, int userId);
         Task<ServiceResponse<GetSessionDto>> UpdateSession (UpdateSessionDto updatedSession);
    }
}