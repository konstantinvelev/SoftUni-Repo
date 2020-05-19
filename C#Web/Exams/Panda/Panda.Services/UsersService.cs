using Panda.Data;
using Panda.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Panda.Services
{
    public class UsersService : IUsersService
    {
        private readonly PandaDbContex db;

        public UsersService(PandaDbContex db)
        {
            this.db = db;
        }

        public string CreateUser(string username, string email, string password)
        {
            var user = new User
            {
                Username = username,
                Email = email,
                Password = HashPassword(password),
            };

            this.db.Users.Add(user);
            this.db.SaveChanges();

            return user.Id;
        }

        public User GetUser(string username, string password)
        {
            var user = this.db.Users.FirstOrDefault(s => s.Username == username && s.Password == HashPassword(password));

            return user;
        }

        public IEnumerable<string> GetUsernames()
        {
            var usernames = this.db.Users.Select(s => s.Username).ToList();
            return usernames;
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                return Encoding.UTF8.GetString(sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password)));
            }
        }
    }
}
