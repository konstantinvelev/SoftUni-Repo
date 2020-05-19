using IRunes.App.ViewModels.Home;
using IRunes.Services;
using SIS.HTTP;
using SIS.MvcFramework;

namespace IRunes.App.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUsersService usersService;

        public HomeController(IUsersService usersService)
        {
            this.usersService = usersService;
        }

        [HttpGet(url: "/")]
        public HttpResponse IndexSlash()
        {
            return this.Index();
        }

        public HttpResponse Index()
        {
             var username = new UsernameViewModel { Username = this.usersService.GetUsernameById(User)};
        

            if (IsUserLoggedIn())
            {
                return this.View(username, "/Home");
            }
            return this.View();
        }
    }
}
