namespace Andreys.App.Controllers
{
    using Andreys.Services;
    using SIS.HTTP;
    using SIS.MvcFramework;

    public class HomeController : Controller
    {
        private readonly IProductService productService;

        public HomeController(IProductService productService)
        {
            this.productService = productService;
        }

        [HttpGet("/")]
        public HttpResponse IndexSlash()
        {
            return this.Index();
        } 

        public HttpResponse Index()
        {
            if (!IsUserLoggedIn())
            {
                return this.View();
            }

            var products = this.productService.All();

            return this.View( "/Home");
        }
    }
}
