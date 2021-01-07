using System.Threading.Tasks;
using CESManager.Models;

namespace CESManager.Data
{
    public interface IAuthRepository
    {
         Task<ServiceResponse<int>> Register (User user, string password);
         Task<ServiceResponse<string>> Login (string username, string password);
         Task<bool> UserExists (string username);
    }
}