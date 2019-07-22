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
            // Enum is defined in CourseWithGradeViewModel.cs
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

            // for each unique pair, creates a courseWithGradeViewModel with CourseInformation and random grade value
            var resultGroup = from item in results
                              select new CourseWithGradeViewModel()
                              {
                                  CRN = item.Course.CRN,
                                  CourseName = item.Course.CourseName,
                                  DepartmentName = item.Course.Department.Name,
                                  Grade = (Grade)grades.GetValue(new Random().Next(grades.Length))
                              };

            // the idea was to get a count of successful(grade higher than I) students for each CRN
            var passedCount = from item in resultGroup
                              where item.Grade <= Grade.I
                              group item by new
                              {
                                  item.CRN
                              }
                              into grp
                              select new
                              {
                                  passed = grp.Count(),
                                  CRN = grp.Key.CRN
                              };

            return resultGroup.ToList();
        }
    }
}
