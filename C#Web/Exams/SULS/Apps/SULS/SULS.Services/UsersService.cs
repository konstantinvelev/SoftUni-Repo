using SULS.Data;
using SULS.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SULS.Services
{
    public class UsersService : IUsersService
    {
        private readonly SULSContext db;

        public UsersService(SULSContext db)
        {
            this.db = db;
        }

        public User GetUser(string username, string password)
        {
            var user = this.db.Users.FirstOrDefault(s => s.Username == username && s.Password == HashPassword(password));

            return user;
        }

        public void Register(string username, string email, string password)
        {
            var user = new User
            {
                Username = username,
                Password = HashPassword(password),
                Email = email
            };

            this.db.Users.Add(user);
            this.db.SaveChanges();
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
