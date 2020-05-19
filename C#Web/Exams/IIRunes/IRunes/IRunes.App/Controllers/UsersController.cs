using IRunes.App.ViewModels;
using IRunes.App.ViewModels.Home;
using IRunes.Services;
using SIS.HTTP;
using SIS.MvcFramework;
using System;

namespace IRunes.App.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUsersService usersService;

        public UsersController(IUsersService usersService)
        {
            this.usersService = usersService;
        }

        public HttpResponse Login()
        {
            return this.View();
        }
        [HttpPost]
        public HttpResponse Login(LoginInputModel inputModel)
        {
            var user = this.usersService.GetUser(inputModel.Username,inputModel.Password);
            if (user == null)
            {
                return this.Redirect("/Users/Login");
            }
            this.SignIn(user.Id);
            return this.Redirect("/");
        }

        public HttpResponse Register()
        {
            return this.View();
        }

        [HttpPost]
        public HttpResponse Register(RegisterInputModel input)
        {
            if (input.Password != input.confirmPassword)
            {
                return this.Redirect("/Users/Register");
            }
            if (input.Username.Length < 4 || input.Username.Length > 10)
            {
                return this.Redirect("/Users/Register");
            }
            if (input.Password.Length < 6 || input.Password.Length > 20)
            {
                return this.Redirect("/Users/Register");
            }
            if (string.IsNullOrWhiteSpace(input.Email))
            {
                return this.Redirect("/Users/Register");
            }

            this.usersService.Register(input.Username,input.Password,input.Email);

           return this.Redirect("/Users/Login");
        }

        public HttpResponse Logout()
        {
            this.SignOut();
            return this.Redirect("/Home/Index");
        }
    }
}
