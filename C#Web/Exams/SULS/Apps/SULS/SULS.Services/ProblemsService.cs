using System.Collections.Generic;
using System.Linq;
using SULS.App.ViewModels.Problems;
using SULS.Data;
using SULS.Models;

namespace SULS.Services
{
    public class ProblemsService : IProblemsService
    {
        private readonly SULSContext db;

        public ProblemsService(SULSContext db)
        {
            this.db = db;
        }

        public List<ProblemsInfoViewModel> AllProblems()
        {
            var countOfProblems = this.db.Problems.Count();

           var problems = this.db.Problems.Select(s => new ProblemsInfoViewModel
            {
                Id= s.Id,
                Name= s.Name,
                Count = countOfProblems
           })
                .ToList();

            return problems;
        }

        public void Create(string name, int points)
        {
            var problem = new Problem
            {
                Name = name,
                Points = points
            };

            this.db.Problems.Add(problem);
            this.db.SaveChanges();
        }

        public Problem GetProblem(string id)
        {
            var problem = this.db.Problems.Find(id);
            return problem;
        }
    }
}
