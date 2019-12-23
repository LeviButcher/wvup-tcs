using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tcs_service.Models;
using tcs_service.Models.ViewModels;

namespace tcs_service.Services
{
    public class ReportsBusinessLogic
    {

        public static List<WeeklyVisitsViewModel> WeeklyVisits(IEnumerable<Session> sessions, DateTime start, DateTime end)
        {
            var result = new List<WeeklyVisitsViewModel>();
            while (start <= end)
            {
                result.Add(new WeeklyVisitsViewModel(start, start.Date.AddDays(6))
                {
                    Count = sessions.Where(x => x.InTime >= start && x.InTime <= start.AddDays(6)).Count()
                });
                start = start.AddDays(7);
            }
            return result;
        }

        public static List<PeakHoursViewModel> PeakHours(List<Session> sessions, DateTime start, DateTime end)
        {
            return null;
        }

        public static List<CourseWithSuccessCountViewModel> SuccessReport(List<CourseWithGradeViewModel> classesWithGrades)
        {

            List<CourseWithSuccessCountViewModel> coursesWithSuccessCount = new List<CourseWithSuccessCountViewModel>();

            foreach (var course in classesWithGrades)
            {
                CourseWithSuccessCountViewModel successCount = null;

                if (coursesWithSuccessCount.Any(x => x.CRN == course.CRN))
                {
                    successCount = coursesWithSuccessCount.Where(x => x.CRN == course.CRN).First();
                }
                if (successCount != null)
                {
                    DetermineSuccess(course.Grade, successCount);
                }
                else
                {
                    var successCourse = new CourseWithSuccessCountViewModel()
                    {
                        ClassName = course.CourseName,
                        CRN = course.CRN,
                        DepartmentName = course.DepartmentName
                    };

                    DetermineSuccess(course.Grade, successCourse);
                    coursesWithSuccessCount.Add(successCourse);
                }
            }

            return coursesWithSuccessCount;
        }

        private static void DetermineSuccess(Grade grade, CourseWithSuccessCountViewModel vm)
        {
            if (grade <= Grade.I)
            {
                vm.PassedSuccessfullyCount++;
                vm.CompletedCourseCount++;
            }
            else if (grade <= Grade.F)
            {
                vm.CompletedCourseCount++;
            }
            else
            {
                vm.DroppedStudentCount++;
            }

            vm.UniqueStudentCount++;
        }
    }


   
}
