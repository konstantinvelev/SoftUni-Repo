using SIS.HTTP.Responses;
using SIS.MvcFramework;
using SIS.MvcFramework.Attributes;
using SULS.App.ViewModels.Home;
using SULS.App.ViewModels.Problems;
using SULS.Services;
using System.Collections.Generic;

namespace SULS.App.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProblemsService problemsService;

        public HomeController(IProblemsService problemsService)
        {
            this.problemsService = problemsService;
        }

        [HttpGet(Url = "/")]
        public HttpResponse HomeSlash()
        {
            return this.Index();
        }


        [HttpGet(Url = "/Home/Index")]
        public HttpResponse Index()
        {
            var problems = new List<DtoViewModel>();

            foreach (var item in this.problemsService.AllProblems())
            {
                var dto = new DtoViewModel
                {
                    Name = item.Name,
                    Count = item.Count,
                    Id = item.Id
                };
                problems.Add(dto);
            }


            var viewMdoel = new AllProblemsViewModel
            {
                Problems = problems
            };

            if (IsLoggedIn())
            {
                return this.View(viewMdoel, "/IndexLoggedIn");
            }

            return this.View();
        }

    }
}