using AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tcs_service.EF;
using tcs_service.Models;
using tcs_service.Models.ViewModels;
using tcs_service.Services;
using tcs_service_test.Helpers;
using Xunit;

namespace tcs_service_test.Services
{
    public class ReportBusinessLogicTest : IDisposable
    {
        readonly string dbName = "ReportRepoTest";
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
        public void Success_StudentsSummedCorrectly()
        {
            var courseWithGradeList = new List<CourseWithGradeViewModel>();

            var history = new Class() { CRN = 555, Name = "History", ShortName = "Hist" };
            var art = new Class() { CRN = 121, Name = "Art", ShortName = "Art" };
            var english = new Class() { CRN = 101, Name = "English", ShortName = "Engl" };
            var redDept = new Department() { Code = 111, Name = "Red" };
            var greenDept = new Department() { Code = 222, Name = "Green" };
            var blueDept = new Department() { Code = 222, Name = "Blue" };

            var classList = new List<Class>();
            classList.Add(history);
            classList.Add(art);
            classList.Add(english);
            var departmentList = new List<Department>();
            departmentList.Add(redDept);
            departmentList.Add(greenDept);
            departmentList.Add(blueDept);

            for (int i = 0; i < 100; i++)
            {
                Random rand = new Random();
                var num = rand.Next(3);
                var courseWithGrade = fixture.Create<CourseWithGradeViewModel>();
                courseWithGrade.CRN = classList[num].CRN;
                courseWithGrade.CourseName = classList[num].Name;
                courseWithGrade.DepartmentName = departmentList[num].Name;
                courseWithGradeList.Add(courseWithGrade);
            }

            var results = ReportsBusinessLogic.SuccessReport(courseWithGradeList);

            var historyGrades = courseWithGradeList.Where(x => x.CRN == 555);
            var passedSuccessfully = historyGrades.Where(x => x.Grade == Grade.A || x.Grade == Grade.B || x.Grade == Grade.C || x.Grade == Grade.I);

            Assert.Equal(passedSuccessfully.Count(), results.Where(x => x.CRN == 555).FirstOrDefault().PassedSuccessfullyCount);
            
        }
    }
}
