using IRunes.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace IRunes.Services
{
    public interface IUsersService
    {
        void Register(string username, string password, string email);

        User GetUser(string username, string password);

        string GetUsernameById(string id);
    }
}
