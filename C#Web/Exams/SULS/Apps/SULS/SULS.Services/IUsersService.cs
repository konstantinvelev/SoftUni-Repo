using SULS.Models;

namespace SULS.Services
{
   public interface IUsersService
    {
        void Register(string username,string email,string password);

        User GetUser(string username, string password);
    }
}
