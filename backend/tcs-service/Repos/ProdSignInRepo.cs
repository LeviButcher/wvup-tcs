using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tcs_service.EF;
using tcs_service.Models.ViewModels;

namespace tcs_service.Repos
{
    public class ProdSignInRepo : SignInRepo
    {
        public ProdSignInRepo(DbContextOptions options) : base(options) { }
        public override StudentInfoViewModel GetStudentInfoWithEmail(string studentEmail)
        {
            throw new NotImplementedException();
        }

        public override StudentInfoViewModel GetStudentInfoWithID(int studentID)
        {
            throw new NotImplementedException();
        }
    }
}
