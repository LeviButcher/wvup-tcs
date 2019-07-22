using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tcs_service.Models.ViewModels;

namespace tcs_service.Repos
{
    public class ProdReportsRepo : ReportsRepo
    {
        public ProdReportsRepo(DbContextOptions options) : base(options) { }

        public override async Task<List<CourseWithGradeViewModel>> SuccessReport(int semesterId)
        {
            throw new NotImplementedException();
        }
    }
}
