using SIS.HTTP.Responses;
using SIS.MvcFramework;
using SIS.MvcFramework.Attributes;
using SULS.App.ViewModels.Problems;
using SULS.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SULS.App.Controllers
{
   public class ProblemsController : Controller
    {
        private readonly IProblemsService problemsService;

        public ProblemsController(IProblemsService problemsService)
        {
            this.problemsService = problemsService;
        }

        public HttpResponse Create()
        {
            if (!IsLoggedIn())
            {
                return this.Redirect("/Users/Login");
            }
            return this.View();
        }

        [HttpPost]
        public HttpResponse Create(string name, int points)
        {
            if (!IsLoggedIn())
            {
                return this.Redirect("/Users/Login");
            }
            if (name.Length < 5 || name.Length > 20)
            {
                return this.Redirect("/Problems/Create");
            }
            if (points < 50 || points > 300)
            {
                return this.Redirect("/Problems/Create");
            }

            this.problemsService.Create(name, points); 
            return this.Redirect("/");
        }
        
        public HttpResponse Details(string id)
        {
            var problem = this.problemsService.GetProblem(id);
            var viewModel = new DetailsProblemViewModel()
            {
                Id = id,
                
            }
            return this.View();
        }
    }
}
