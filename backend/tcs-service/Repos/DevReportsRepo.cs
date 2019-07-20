using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tcs_service.Models.ViewModels;

namespace tcs_service.Repos
{
    public class DevReportsRepo : ReportsRepo
    {
        public DevReportsRepo(DbContextOptions options) : base(options) { }
        
        public override async Task<List<CourseWithSuccessCountViewModel>> SuccessReport(int semesterID)
        {
            List<CourseWithSuccessCountViewModel> success = new List<CourseWithSuccessCountViewModel>();

            success.Add(new CourseWithSuccessCountViewModel
            {
                ClassName = "History 101",
                CRN = 1411,
                UniqueStudentCount = 54,
                DroppedStudentCount = 4,
                CompletedCourseCount = 50,
                PassedSuccessfullyCount = 45
            });
            return success;
        }
    }
}
