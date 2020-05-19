using SULS.Data;
using SULS.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SULS.Services
{
    public class SubmissionsService : ISubmissionsService
    {
        private readonly SULSContext db;

        public SubmissionsService(SULSContext db)
        {
            this.db = db;
        }

        public void Create(string code, string problemId)
        {

            var submission = new Submission
            {
                Code = code,
                CreatedOn = DateTime.UtcNow,
                AchievedResult = 60
            };
            
        }
    }
}
