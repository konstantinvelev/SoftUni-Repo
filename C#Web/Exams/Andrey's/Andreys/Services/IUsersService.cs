using System;
using System.Collections.Generic;
using System.Text;

namespace Andreys.Services
{
    public interface IUsersService
    {
        string GetUserId(string username, string password);

        void Register(string username, string email, string password);

        string GetUsername(string id);
    }
}