using System;
using System.Collections.Generic;
using System.Linq;
using tcs_service.Models;
using tcs_service.Models.DTO;

namespace tcs_service.Services
{
    public class ReportsBusinessLogic
    {
        public static List<WeeklyVisitsDTO> WeeklyVisits(IEnumerable<Session> sessions, DateTime start, DateTime end)
        {
            var result = new List<WeeklyVisitsDTO>();
            while (start <= end)
            {
                result.Add(new WeeklyVisitsDTO(start, start.Date.AddDays(6))
                {
                    Count = sessions.Where(x => x.InTime.Date >= start.Date && x.InTime.Date <= start.AddDays(6)).Count()
                });
                start = start.AddDays(7);
            }
            return result;
        }

        public static List<PeakHoursDTO> PeakHours(IEnumerable<Session> sessions, DateTime start, DateTime end) => 
                sessions.Where(x => x.InTime.Date >= start.Date && x.InTime.Date <= end.Date)
                .GroupBy(x => x.InTime.Hour)
                .Where(x => x.Count() >= 1)
                .Select(x => new PeakHoursDTO(x.Key, x.Count())).ToList();
        
        public static List<ClassTourReportDTO> ClassTours(IEnumerable<ClassTour> tours, DateTime start, DateTime end) =>
                tours.Where(x => x.DayVisited >= start && x.DayVisited <= end)
                .GroupBy(x => x.Name)
                .Select(x => new ClassTourReportDTO { Name = x.Key, Students = x.Sum(s => s.NumberOfStudents) })
                .ToList();

        public static List<TeacherSignInTimeDTO> Volunteers(IEnumerable<Session> sessions, DateTime start, DateTime end)
        {
            var teachers = from session in sessions
                           where session.InTime.Date >= start.Date
                           && session.InTime.Date <= end.Date
                           && session.Person.PersonType == PersonType.Teacher
                           select new
                           {
                               fullName = $"{session.Person.FirstName} {session.Person.LastName}",
                               teacherEmail = session.Person.Email,
                               totalHours = Convert.ToDecimal(session.OutTime.Value.Ticks) - Convert.ToDecimal(session.InTime.Ticks)
                           };


            var result = from item in teachers
                         group item by new
                         {
                             item.teacherEmail,
                             item.fullName
                         }
                         into grp
                         select new TeacherSignInTimeDTO()
                         {
                             FullName = grp.Key.fullName,
                             TeacherEmail = grp.Key.teacherEmail,
                             TotalHours = Math.Round(grp.Sum(x => x.totalHours / 36000000000), 2)  //36000000000 = microseconds in an hour
                         };

            return result.ToList();
        }

        public static List<ReasonWithClassVisitsDTO> Reasons(IEnumerable<Session> sessions, DateTime start, DateTime end)
        {
            var result = from session in sessions
                         from reason in session.SessionReasons
                         where reason.Reason.Name != "Tutoring"
                         from course in session.SessionClasses
                         where session.InTime.Date >= start.Date
                         && session.InTime.Date <= end.Date
                         select new
                         {
                             ReasonName = reason.Reason.Name,
                             reason.ReasonId,
                             className = course.Class.Name,
                             classId = course.ClassId
                         };

            var tutoringResult = from session in sessions
                                 from reason in session.SessionReasons
                                 from course in session.SessionClasses
                                 where session.Tutoring == true
                                 && session.InTime.Date >= start.Date
                                 && session.InTime.Date <= end.Date
                                 select new
                                 {

                                     className = course.Class.Name,
                                     classId = course.ClassId
                                 };

            var tutoringWithoutReasonResult = from session in sessions
                                 from course in session.SessionClasses
                                 where session.Tutoring == true
                                 && session.InTime.Date >= start.Date
                                 && session.InTime.Date <= end.Date
                                 select new
                                 {
                                     className = course.Class.Name,
                                     classId = course.ClassId
                                 };


            var resultGroup = from item in result
                              group item by new
                              {
                                  item.classId,
                                  item.ReasonId,
                                  item.className,
                                  item.ReasonName
                              } into grp
                              select new ReasonWithClassVisitsDTO()
                              {
                                  ReasonId = grp.Key.ReasonId,
                                  ReasonName = grp.Key.ReasonName,
                                  ClassCRN = grp.Key.classId,
                                  ClassName = grp.Key.className,
                                  Visits = grp.Count()
                              };

            var tutorResult = from item in tutoringResult
                              group item by new
                              {
                                  item.classId,
                                  item.className
                              } into grp
                              select new ReasonWithClassVisitsDTO()
                              {
                                  ReasonId = 0,
                                  ReasonName = "Tutoring",
                                  ClassCRN = grp.Key.classId,
                                  ClassName = grp.Key.className,
                                  Visits = grp.Count()
                              };

            var tutorWithoutReason = from item in tutoringWithoutReasonResult
                              group item by new
                              {
                                  item.classId,
                                  item.className
                              } into grp
                              select new ReasonWithClassVisitsDTO()
                              {
                                  ReasonId = 0,
                                  ReasonName = "Tutoring",
                                  ClassCRN = grp.Key.classId,
                                  ClassName = grp.Key.className,
                                  Visits = grp.Count()
                              };
            
            var finalResult = resultGroup.Concat(tutorResult).Concat(tutorWithoutReason);


            return finalResult.Distinct().ToList();
        }

        public static List<ClassWithSuccessCountDTO> SuccessReport(IEnumerable<ClassWithGradeDTO> classesWithGrades)
        {

            List<ClassWithSuccessCountDTO> coursesWithSuccessCount = new List<ClassWithSuccessCountDTO>();

            foreach (var course in classesWithGrades)
            {
                ClassWithSuccessCountDTO successCount = null;

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
                    var successCourse = new ClassWithSuccessCountDTO()
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

        private static void DetermineSuccess(Grade grade, ClassWithSuccessCountDTO vm)
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
