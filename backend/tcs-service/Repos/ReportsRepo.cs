using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tcs_service.EF;
using tcs_service.Helpers;
using tcs_service.Models;
using tcs_service.Models.DTO;
using tcs_service.Repos.Base;
using tcs_service.Repos.Interfaces;
using tcs_service.Services.Interfaces;

namespace tcs_service.Repos
{
    public class ReportsRepo : IReportsRepo
    {
        private readonly TCSContext _db;
        readonly IBannerService _bannerService;
        public ReportsRepo(TCSContext tcs, IBannerService bannerService)
        {
            _db = tcs;
            _bannerService = bannerService;
        }
        
        public async Task<List<WeeklyVisitsDTO>> WeeklyVisits(DateTime startWeek, DateTime endWeek)
        {
            var result = new List<WeeklyVisitsDTO>();
            while (startWeek <= endWeek)
            {
                result.Add(new WeeklyVisitsDTO(startWeek, startWeek.Date.AddDays(6))
                {
                    Count = await _db.Sessions.Where(x => x.InTime >= startWeek && x.InTime <= startWeek.AddDays(6)).CountAsync()
                });
                startWeek = startWeek.AddDays(7);
            }
            return result;
        }

        public async Task<List<PeakHoursDTO>> PeakHours(DateTime startWeek, DateTime endWeek)
             => await _db.Sessions.Where(x => x.InTime >= startWeek && x.InTime <= endWeek)
                .GroupBy(x => x.InTime.Hour)
                .Where(x => x.Count() >= 1)
                .Select(x => new PeakHoursDTO(x.Key, x.Count()))
                .ToListAsync();


        public async Task<List<ClassTourReportDTO>> ClassTours(DateTime startWeek, DateTime endWeek)
        {
            var result = await _db.ClassTours.Where(x => x.DayVisited >= startWeek && x.DayVisited <= endWeek)
                .GroupBy(x => x.Name).Select(x => new ClassTourReportDTO { Name = x.Key, Students = x.Sum(s => s.NumberOfStudents) }).ToListAsync();
            
            return result;
        }

        public async Task<List<TeacherSignInTimeDTO>> Volunteers(DateTime startWeek, DateTime endWeek)
        {
            var teachers = from signIn in _db.Sessions
                           where signIn.InTime >= startWeek
                           && signIn.InTime <= endWeek
                           && signIn.Person.PersonType == PersonType.Teacher
                           select new
                           {
                               fullName = $"{signIn.Person.FirstName} {signIn.Person.LastName}",
                               teacherEmail = signIn.Person.Email,
                               totalHours = Convert.ToDecimal(signIn.OutTime.Value.Ticks) - Convert.ToDecimal(signIn.InTime.Ticks)
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
                             TotalHours = Math.Round(grp.Sum(x => x.totalHours / 600000000) / 60, 2)
                         };

            return await result.ToListAsync();
        }

        public async Task<List<ReasonWithClassVisitsDTO>> Reasons(DateTime startWeek, DateTime endWeek)
        {
            var result = from signIns in _db.Sessions
                         from reason in signIns.SessionReasons
                         where reason.Reason.Name != "Tutoring"
                         from course in signIns.SessionClasses
                         where signIns.InTime >= startWeek
                         && signIns.InTime <= endWeek
                         select new
                         {
                             ReasonName = reason.Reason.Name,
                             ReasonId = reason.ReasonId,
                             CourseName = course.Class.Name,
                             CourseId = course.ClassId
                         };

            var tutoringResult = from signIns in _db.Sessions
                                 from reason in signIns.SessionReasons
                                 from course in signIns.SessionClasses
                                 where signIns.Tutoring == true
                                 && signIns.InTime >= startWeek
                                 && signIns.InTime <= endWeek
                                 select new
                                 {

                                     CourseName = course.Class.Name,
                                     CourseId = course.ClassId
                                 };

            var resultGroup = from item in result
                              group item by new
                              {
                                  item.CourseId,
                                  item.ReasonId,
                                  item.CourseName,
                                  item.ReasonName
                              } into grp
                              select new ReasonWithClassVisitsDTO()
                              {
                                  ReasonId = grp.Key.ReasonId,
                                  ReasonName = grp.Key.ReasonName,
                                  CourseCRN = grp.Key.CourseId,
                                  CourseName = grp.Key.CourseName,
                                  Visits = grp.Count()
                              };

            var tutorResult = from item in tutoringResult
                              group item by new
                              {
                                  item.CourseId,
                                  item.CourseName
                              } into grp
                              select new ReasonWithClassVisitsDTO()
                              {
                                  ReasonId = 0,
                                  ReasonName = "Tutoring",
                                  CourseCRN = grp.Key.CourseId,
                                  CourseName = grp.Key.CourseName,
                                  Visits = grp.Count()
                              };

            var finalResult = resultGroup.Concat(tutorResult);

            return await finalResult.ToListAsync();
        }
      
        public async Task<List<ClassWithGradeDTO>> SuccessReport(int semesterId)
        {
            var studentCourses = from item in _db.Sessions
                                 from course in item.SessionClasses
                                 where item.SemesterCode == semesterId
                                 select new
                                 {
                                     item.PersonId,
                                     course.Class,
                                     course.Class.Department,
                                 };

            List<ClassWithGradeDTO> coursesWithGrades = new List<ClassWithGradeDTO>();

            foreach (var item in studentCourses.Distinct())
            {
                try
                {
                    var grade = await _bannerService.GetStudentGrade(item.PersonId, item.Class.CRN, semesterId);
                    coursesWithGrades.Add(grade);
                }
                catch
                {
                    throw new TCSException("Something went wrong");
                }
            }

            return coursesWithGrades;
            
        }
    }
}
