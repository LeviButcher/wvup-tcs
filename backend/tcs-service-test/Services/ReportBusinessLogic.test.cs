using AutoFixture;
using AutoFixture.AutoMoq;
using System;
using System.Collections.Generic;
using System.Linq;
using tcs_service.EF;
using tcs_service.Models;
using tcs_service.Models.DTO;
using tcs_service.Services;
using tcs_service_test.Helpers;
using Xunit;

namespace tcs_service_test.Services
{
    public class ReportBusinessLogicTest : IDisposable
    {
        readonly string dbName = "ReportBusinessLogicTest";
        readonly IFixture fixture;
        readonly TCSContext db;
       
        public ReportBusinessLogicTest()
        {
            var dbOptions = DbInMemory.getDbInMemoryOptions(dbName);
            db = new TCSContext(dbOptions);
            fixture = new Fixture();
        }

        public void Dispose()
        {
            db.Database.EnsureDeleted();
        }

        [Fact]
        public void WeeklyVisits_SevenSessionsWithinDate_TwoSessionsOutsideOfDate_ResultCountShouldBeSeven()
        {
            var sessions = new List<Session>();
           
            sessions.Add(new Session() { InTime = new DateTime(2020, 12, 23), OutTime = new DateTime(2020, 12, 23) });
            sessions.Add(new Session() { InTime = new DateTime(2020, 12, 24), OutTime = new DateTime(2020, 12, 24) });
            sessions.Add(new Session() { InTime = new DateTime(2020, 12, 25), OutTime = new DateTime(2020, 12, 25) });
            sessions.Add(new Session() { InTime = new DateTime(2020, 12, 26), OutTime = new DateTime(2020, 12, 26) });
            sessions.Add(new Session() { InTime = new DateTime(2020, 12, 27), OutTime = new DateTime(2020, 12, 27) });
            sessions.Add(new Session() { InTime = new DateTime(2020, 12, 28), OutTime = new DateTime(2020, 12, 28) });
            sessions.Add(new Session() { InTime = new DateTime(2020, 12, 29), OutTime = new DateTime(2020, 12, 29) });
            sessions.Add(new Session() { InTime = new DateTime(2020, 12, 30), OutTime = new DateTime(2020, 12, 30) });
            sessions.Add(new Session() { InTime = new DateTime(2020, 12, 31), OutTime = new DateTime(2020, 12, 31) });

            var results = ReportsBusinessLogic.WeeklyVisits(sessions, new DateTime(2020, 12, 24), new DateTime(2020, 12, 30));
            Assert.Equal(7, results.FirstOrDefault().Count);                      
        }

        [Fact]
        public void PeakHours_FiveSessionsAt10AM_TwoSessionsAt5PM()
        {
            var sessions = new List<Session>();

            sessions.Add(new Session() { InTime = new DateTime(2020, 12, 24, 10, 15, 0), OutTime = new DateTime(2020, 12, 24, 10, 35, 0) });
            sessions.Add(new Session() { InTime = new DateTime(2020, 12, 24, 10, 30, 0), OutTime = new DateTime(2020, 12, 24, 10, 35, 0) });
            sessions.Add(new Session() { InTime = new DateTime(2020, 12, 25, 10, 30, 0), OutTime = new DateTime(2020, 12, 25, 10, 45, 0) });
            sessions.Add(new Session() { InTime = new DateTime(2020, 12, 26, 10, 45, 0), OutTime = new DateTime(2020, 12, 23, 10, 55, 0) });
            sessions.Add(new Session() { InTime = new DateTime(2020, 12, 27, 10, 30, 0), OutTime = new DateTime(2020, 12, 23, 10, 35, 0) });

            sessions.Add(new Session() { InTime = new DateTime(2020, 12, 27, 17, 30, 0), OutTime = new DateTime(2020, 12, 23, 17, 35, 0) });
            sessions.Add(new Session() { InTime = new DateTime(2020, 12, 30, 17, 30, 0), OutTime = new DateTime(2020, 12, 30, 17, 35, 0) });

            var results = ReportsBusinessLogic.PeakHours(sessions, new DateTime(2020, 12, 24), new DateTime(2020, 12, 30));
            Assert.Equal(5, results.Where(x => x.Hour == "10 A.M").FirstOrDefault().Count);
            Assert.Equal(2, results.Where(x => x.Hour == "5 P.M").FirstOrDefault().Count);
        }

        [Fact]
        public void ClassTours_OneSchoolWithTwoTours_OneSchoolWithOneTour_SummedCorrectly()
        {
            var tours = new List<ClassTour>();

            tours.Add(new ClassTour() { DayVisited = new DateTime(2020, 12, 24), Name = "Lincoln High School", NumberOfStudents = 25 });
            tours.Add(new ClassTour() { DayVisited = new DateTime(2020, 12, 28), Name = "Lincoln High School", NumberOfStudents = 20 });
            tours.Add(new ClassTour() { DayVisited = new DateTime(2020, 12, 30), Name = "Larry David High School", NumberOfStudents = 35 });

            var results = ReportsBusinessLogic.ClassTours(tours, new DateTime(2020, 12, 24), new DateTime(2020, 12, 30));
            Assert.Equal(45, results.Where(x => x.Name == "Lincoln High School").FirstOrDefault().Students);
            Assert.Equal(35, results.Where(x => x.Name == "Larry David High School").FirstOrDefault().Students);
        }

        [Fact]
        public void Volunteers_OneTeacherWithTwoSessions_OtherTeacherWithOneSession()
        {
            var sessions = new List<Session>();
            var people = new List<Person>();
            people.Add(new Person() { PersonType = PersonType.Teacher, Email = "teacher1@wvup.edu", FirstName = "Teacher", LastName = "One", Id = 12345 });
            people.Add(new Person() { PersonType = PersonType.Teacher, Email = "teacher2@wvup.edu", FirstName = "Teacher", LastName = "Two", Id = 45678 });
            db.People.AddRange(people);
            db.SaveChanges();

            sessions.Add(new Session()
            {
                InTime = new DateTime(2020, 12, 24, 10, 0, 0),
                OutTime = new DateTime(2020, 12, 24, 11, 00, 0),
                PersonId = 12345
            });

            sessions.Add(new Session()
            {
                InTime = new DateTime(2020, 12, 24, 12, 0, 0),
                OutTime = new DateTime(2020, 12, 24, 15, 00, 0),
                PersonId = 12345
            });

            sessions.Add(new Session()
            {
                InTime = new DateTime(2020, 12, 24, 9, 0, 0),
                OutTime = new DateTime(2020, 12, 24, 15, 0, 0),
                PersonId = 45678
            });
            db.Sessions.AddRange(sessions);
            db.SaveChanges();

            var results = ReportsBusinessLogic.Volunteers(sessions, new DateTime(2020, 12, 24), new DateTime(2020, 12, 30));
            Assert.Equal(4, results.Where(x => x.TeacherEmail == "teacher1@wvup.edu").FirstOrDefault().TotalHours);
            Assert.Equal(6, results.Where(x => x.TeacherEmail == "teacher2@wvup.edu").FirstOrDefault().TotalHours);
        }

        [Fact]
        public void Reasons_FiveSessionsAt10AM_TwoSessionsAt5PM()
        {
            var reasons = new List<Reason>();
            reasons.Add(new Reason() { Name = "Study Time", Deleted = false });
            reasons.Add(new Reason() { Name = "Computer Use", Deleted = false });
            reasons.Add(new Reason() { Name = "Printer Use", Deleted = false });
            db.Reasons.AddRange(reasons);
            db.SaveChanges();

            db.Departments.Add(new Department() { Code = 111, Name = "General " });
            db.SaveChanges();

            var classes = new List<Class>();
            classes.Add(new Class() { Name = "Art 101", CRN = 123, ShortName = "Art", DepartmentCode = 111 });
            db.Classes.AddRange(classes);
            db.SaveChanges();

            db.People.Add( new Person() { PersonType = PersonType.Student, Email = "student1@wvup.edu", FirstName = "Student", LastName = "One", Id = 12345 });
            db.SaveChanges();

            var sessions = new List<Session>();
            sessions.Add(new Session()
            {
                InTime = new DateTime(2020, 12, 24, 10, 0, 0),
                OutTime = new DateTime(2020, 12, 24, 11, 00, 0),
                Tutoring = true,
                PersonId = 12345
            });
            db.Sessions.AddRange(sessions);
            db.SaveChanges();
            var sessionId = db.Sessions.Where(x => x.PersonId == 12345).FirstOrDefault().Id;

            db.SessionClasses.Add(new SessionClass() { ClassId = 123, SessionId = sessionId });
            db.SaveChanges();

            var studyTime = db.Reasons.Where(x => x.Name == "Study Time").FirstOrDefault();
            var computerUse = db.Reasons.Where(x => x.Name == "Computer Use").FirstOrDefault();
            var printerUse = db.Reasons.Where(x => x.Name == "Printer Use").FirstOrDefault();
            db.SessionReasons.Add(new SessionReason() { SessionId = sessionId, ReasonId = studyTime.Id });
            db.SaveChanges();

            var results = ReportsBusinessLogic.Reasons(sessions, new DateTime(2020, 12, 24), new DateTime(2020, 12, 30));
            Assert.Equal(1, results.Where(x => x.ReasonId== studyTime.Id).FirstOrDefault().Visits);
        }

        [Fact]
        public void SuccessReport_GradesGeneratedWithFixture_SuccessReportSummedCorrectly()
        {
            var courseWithGradeList = new List<ClassWithGradeDTO>();
            
            for (int i = 0; i < 100; i++)
            {
                var courseWithGrade = fixture.Create<ClassWithGradeDTO>();
                courseWithGrade.CRN = 555;
                courseWithGrade.CourseName = "history";
                courseWithGrade.DepartmentName = "History";
                courseWithGradeList.Add(courseWithGrade);
            }

            var results = ReportsBusinessLogic.SuccessReport(courseWithGradeList);

            var historyGrades = courseWithGradeList.Where(x => x.CRN == 555);
            var passedSuccessfully = historyGrades.Where(x => x.Grade == Grade.A || x.Grade == Grade.B || x.Grade == Grade.C || x.Grade == Grade.I);
            var completedClass = historyGrades.Where(x => x.Grade == Grade.A || x.Grade == Grade.B || x.Grade == Grade.C || x.Grade == Grade.I || x.Grade == Grade.D || x.Grade == Grade.F);
            var droppedClass = historyGrades.Where(x => x.Grade == Grade.W || x.Grade == Grade.FIW);

            Assert.Equal(passedSuccessfully.Count(), results.Where(x => x.CRN == 555).FirstOrDefault().PassedSuccessfullyCount);
            Assert.Equal(completedClass.Count(), results.Where(x => x.CRN == 555).FirstOrDefault().CompletedCourseCount);
            Assert.Equal(droppedClass.Count(), results.Where(x => x.CRN == 555).FirstOrDefault().DroppedStudentCount);
            Assert.Equal(100, results.Where(x => x.CRN == 555).FirstOrDefault().UniqueStudentCount);
        }

        [Fact]
        public void SuccessReport_GradesEnteredManually_SuccessReportSummedCorrectly()
        {
            var courseWithGradeList = new List<ClassWithGradeDTO>();
            courseWithGradeList.Add(new ClassWithGradeDTO() { CourseName = "History", CRN = 555, DepartmentName = "History Dept.", Grade = Grade.A });
            courseWithGradeList.Add(new ClassWithGradeDTO() { CourseName = "History", CRN = 555, DepartmentName = "History Dept.", Grade = Grade.B });
            courseWithGradeList.Add(new ClassWithGradeDTO() { CourseName = "History", CRN = 555, DepartmentName = "History Dept.", Grade = Grade.B });
            courseWithGradeList.Add(new ClassWithGradeDTO() { CourseName = "History", CRN = 555, DepartmentName = "History Dept.", Grade = Grade.C });
            courseWithGradeList.Add(new ClassWithGradeDTO() { CourseName = "History", CRN = 555, DepartmentName = "History Dept.", Grade = Grade.I });

            courseWithGradeList.Add(new ClassWithGradeDTO() { CourseName = "History", CRN = 555, DepartmentName = "History Dept.", Grade = Grade.D });
            courseWithGradeList.Add(new ClassWithGradeDTO() { CourseName = "History", CRN = 555, DepartmentName = "History Dept.", Grade = Grade.D });
            courseWithGradeList.Add(new ClassWithGradeDTO() { CourseName = "History", CRN = 555, DepartmentName = "History Dept.", Grade = Grade.F });

            courseWithGradeList.Add(new ClassWithGradeDTO() { CourseName = "History", CRN = 555, DepartmentName = "History Dept.", Grade = Grade.W });
            courseWithGradeList.Add(new ClassWithGradeDTO() { CourseName = "History", CRN = 555, DepartmentName = "History Dept.", Grade = Grade.FIW });


            var results = ReportsBusinessLogic.SuccessReport(courseWithGradeList);
            
            Assert.Equal(5, results.Where(x => x.CRN == 555).FirstOrDefault().PassedSuccessfullyCount);
            Assert.Equal(8, results.Where(x => x.CRN == 555).FirstOrDefault().CompletedCourseCount);
            Assert.Equal(2, results.Where(x => x.CRN == 555).FirstOrDefault().DroppedStudentCount);
            Assert.Equal(10, results.Where(x => x.CRN == 555).FirstOrDefault().UniqueStudentCount);
        }
    }
}
