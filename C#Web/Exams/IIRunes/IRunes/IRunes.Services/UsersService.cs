using IRunes.Data;
using IRunes.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace IRunes.Services
{
    public class UsersService : IUsersService
    {
        private readonly RunesDbContext db;

        public UsersService(RunesDbContext db)
        {
            this.db = db;
        }

        public User GetUser(string username, string password)
        {
           var user  = this.db.Users.FirstOrDefault(s=>s.Username == username && s.Password == HashPassword(password));

            return user;
        }

        public string GetUsernameById(string id)
        {
            var username = this.db.Users.Where(s => s.Id == id).Select(e => e.Username).FirstOrDefault();

            return username;
        }

        public void Register(string username, string password,  string email)
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
