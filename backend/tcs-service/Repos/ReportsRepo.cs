using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tcs_service.Models;
using tcs_service.Models.ViewModels;
using tcs_service.Repos.Base;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Repos
{
    public abstract class ReportsRepo : BaseRepo<SignIn>, IReportsRepo
    {
        public ReportsRepo(DbContextOptions options) : base(options)
        {

        }

        public override Task<bool> Exist(int id)
        {
            throw new NotImplementedException();
        }

        public override Task<SignIn> Find(int id)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<SignIn> GetAll()
        {
            throw new NotImplementedException();
        }

        public override Task<SignIn> Remove(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ReportCountViewModel>> WeeklyVisits(DateTime startWeek, DateTime endWeek)
        {
            var result = new List<ReportCountViewModel>();
            var count = 1;
            while (startWeek <= endWeek)
            {
                result.Add(new ReportCountViewModel
                {
                    Item = count,
                    Count = await _db.SignIns.Where(x => x.InTime >= startWeek && x.InTime <= startWeek.AddDays(7)).CountAsync()
                });
                count++;
                startWeek = startWeek.AddDays(7);
            }
            return result;
        }

        public async Task<List<ReportCountViewModel>> PeakHours(DateTime startWeek, DateTime endWeek)
        {
            var result = new List<ReportCountViewModel>();
            var realResult = new List<ReportCountViewModel>();
            var count = 0;

            while (startWeek <= endWeek)
            {
                while (count <= 23)
                {
                    result.Add(new ReportCountViewModel
                    {
                        Item = count,
                        Count = await _db.SignIns.Where(x => x.InTime >= startWeek && x.InTime <= startWeek.AddDays(7) && x.InTime.Value.Hour == count).CountAsync()
                    });
                    count++;
                }

                count = 0;
                startWeek = startWeek.AddDays(7);
            }

            var hourCount = 0;
            var inCount = 0;
            while (hourCount < 24)
            {
                foreach (ReportCountViewModel rc in result)
                {
                    if (rc.Item == hourCount)
                    {
                        inCount += rc.Count;
                    }
                }

                realResult.Add(new ReportCountViewModel
                {
                    Item = hourCount,
                    Count = inCount
                });
                hourCount++;
                inCount = 0;
            }

            return realResult;
        }

        public async Task<List<ClassTourReportViewModel>> ClassTours(DateTime startWeek, DateTime endWeek)
        {
            var result = await _db.ClassTours.Where(x => x.DayVisited >= startWeek && x.DayVisited <= endWeek.AddDays(1))
                .GroupBy(x => x.Name).Select(x => new ClassTourReportViewModel { Name = x.Key, Students = x.Sum(s => s.NumberOfStudents) }).ToListAsync();

            return result;
        }

        public async Task<List<TeacherSignInTimeViewModel>> Volunteers(DateTime startWeek, DateTime endWeek)
        {
            var teachers = from signIn in _db.SignIns
                           where signIn.InTime >= startWeek
                           && signIn.InTime <= endWeek
                           && signIn.Person.PersonType == PersonType.Teacher
                           select new
                           {
                               teacherName = $"{signIn.Person.FirstName} {signIn.Person.LastName}",
                               teacherEmail = signIn.Person.Email,
                               totalHours = signIn.OutTime.Value.Ticks - signIn.InTime.Value.Ticks
                           };


            var result = from item in teachers
                         group item by new
                         {
                             item.teacherEmail,
                             item.teacherName
                         }
                         into grp
                         select new TeacherSignInTimeViewModel()
                         {
                             teacherName = grp.Key.teacherName,
                             teacherEmail = grp.Key.teacherEmail,
                             totalHours = new TimeSpan(grp.Sum(x => x.totalHours))
                         };

            return await result.ToListAsync();
        }


        public async Task<List<ReasonWithClassVisitsViewModel>> Reasons(DateTime startWeek, DateTime endWeek)
        {
            var result = from signIns in _db.SignIns
                         from reason in signIns.Reasons
                         where reason.Reason.Name != "Tutoring"
                         from course in signIns.Courses
                         where signIns.InTime >= startWeek
                         && signIns.InTime <= endWeek
                         select new
                         {
                             ReasonName = reason.Reason.Name,
                             ReasonId = reason.ReasonID,
                             CourseName = course.Course.CourseName,
                             CourseId = course.CourseID
                         };

            var tutoringResult = from signIns in _db.SignIns
                                 from reason in signIns.Reasons
                                 from course in signIns.Courses
                                 where signIns.Tutoring == true
                                 && signIns.InTime >= startWeek
                                 && signIns.InTime <= endWeek
                                 select new
                                 {
   
                                     CourseName = course.Course.CourseName,
                                     CourseId = course.CourseID
                                 };

            var resultGroup = from item in result
                              group item by new
                              {
                                  item.CourseId,
                                  item.ReasonId,
                                  item.CourseName,
                                  item.ReasonName
                              } into grp
                              select new ReasonWithClassVisitsViewModel()
                              {
                                  reasonId = grp.Key.ReasonId,
                                  reasonName = grp.Key.ReasonName,
                                  courseCRN = grp.Key.CourseId,
                                  courseName = grp.Key.CourseName,
                                  visits = grp.Count()
                              };

            var tutorResult = from item in tutoringResult
                              group item by new
                              {
                                  item.CourseId,
                                  item.CourseName
                              } into grp
                              select new ReasonWithClassVisitsViewModel()
                              {
                                  reasonId = 0,
                                  reasonName = "Tutoring",
                                  courseCRN = grp.Key.CourseId,
                                  courseName = grp.Key.CourseName,
                                  visits = grp.Count()
                              };

            var finalResult = resultGroup.Concat(tutorResult);

            return await finalResult.ToListAsync();
        }
        public List<Semester> Semesters()
        {
            return _db.Semesters.ToList();
        }

        public abstract Task<List<CourseWithSuccessCountViewModel>> SuccessReport(int semesterId);
    }
}
