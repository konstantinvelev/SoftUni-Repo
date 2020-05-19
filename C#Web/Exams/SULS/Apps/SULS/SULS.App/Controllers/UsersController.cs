using SIS.HTTP.Responses;
using SIS.MvcFramework;
using SIS.MvcFramework.Attributes;
using SULS.App.ViewModels.Users;
using SULS.Services;

namespace SULS.App.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUsersService usersService;

        public UsersController(IUsersService usersService)
        {
            this.usersService = usersService;
        }
        public HttpResponse Register()
        {
            return this.View();
        }

        [HttpPost]
        public HttpResponse Register(RegisterUserInputModel register)
        {
            if (register.Username.Length<5 || register.Username.Length > 20)
            {
                return this.Redirect("/Users/Register");
            }
            if (string.IsNullOrWhiteSpace(register.Email))
            {
                return this.Redirect("/Users/Register");
            }
            if (register.Password.Length < 6 || register.Password.Length > 20)
            {
                return this.Redirect("/Users/Register");
            }
            if (register.Password != register.ConfirmPassword)
            {
                return this.Redirect("/Users/Register");
            }

            this.usersService.Register(register.Username, register.Email, register.Password);

            return this.Redirect("/Users/Login");
        }

        public HttpResponse Login()
        {
            return this.View();
        }

        [HttpPost]
        public HttpResponse Login(LoginUserInputModel login)
        {
            if (login.Username.Length < 5 || login.Username.Length > 20)
            {
                return this.Redirect("/Users/Login");
            }
            if (login.Password.Length < 6 || login.Password.Length > 20)
            {
                return this.Redirect("/Users/Login");
            }

            var user = this.usersService.GetUser(login.Username, login.Password);

            this.SignIn(user.Id, user.Username, user.Email);

            return this.Redirect("/");
        }

        public HttpResponse Logout()
        {
            this.SignOut();
            return this.Redirect("/");
        }
    }
}