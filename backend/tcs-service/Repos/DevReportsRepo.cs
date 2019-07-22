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
        
        public override async Task<List<CourseWithGradeViewModel>> SuccessReport(int semesterID)
        {
            // Enum is defined in CourseWithGradeViewModel.cs--copied and pasted for convenience
            //A = 1,
            //B = 2,
            //C = 3,
            //I = 4,
            //D = 5,
            //F = 6,
            //W = 7,
            //FIW = 8

            var grades = Enum.GetValues(typeof(Grade));
            
            // gets each signIn with unique pair of personID & CRN 
            var results = from item in _db.SignIns
                          from course in item.Courses
                          where item.SemesterId == semesterID
                          select new
                          {
                              item.PersonId,
                              course.Course
                          };

            // for each unique pair, creates a courseWithGradeViewModel with Course Information and random grade value
            var resultGroup = from item in results
                              select new CourseWithGradeViewModel()
                              {
                                  CRN = item.Course.CRN,
                                  CourseName = item.Course.CourseName,
                                  DepartmentName = item.Course.Department.Name,
                                  Grade = (Grade)grades.GetValue(new Random().Next(grades.Length))
                              };


            var successCount = from item in resultGroup
                               group item by new
                               {
                                   item.CourseName,
                                   item.CRN,
                                   item.DepartmentName
                               }
                               into grp
                               select new CourseWithSuccessCountViewModel()
                               {
                                   ClassName = grp.Key.CourseName,
                                   CRN = grp.Key.CRN,
                                   DepartmentName = grp.Key.DepartmentName,
                                   UniqueStudentCount = grp.Count(),
                                   PassedSuccessfullyCount = GetPassed(resultGroup, grp.Key.CRN)
                               };


            // using this to determine number of passing/unique students for each CRN
            var iWishIWereDead = successCount.ToList();

            // Comparing ^^ to the output of this, and checking if it matches and it never does :-/ 
            return resultGroup.ToList();
        }

        private int GetPassed(IQueryable<CourseWithGradeViewModel> results, int crn)
        {
            var passed = 0;
            foreach(var r in results)
            {
                // if i'm not completely insane, <= Grade.I should be 1, 2, 3, & 4 (A, B, C, I)
                if (r.CRN == crn )
                {
                    if((int)r.Grade <= (int)Grade.I)
                    {
                        passed++;
                    }
                }
            }
            return passed;
        }
    }
}
