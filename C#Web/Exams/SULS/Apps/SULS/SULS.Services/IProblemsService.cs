using SULS.App.ViewModels.Problems;
using SULS.Models;
using System.Collections.Generic;

namespace SULS.Services
{
    public interface IProblemsService
    {
        Problem GetProblem(string id);
        List<ProblemsInfoViewModel> AllProblems();

        void Create(string name, int points);
    }
}
