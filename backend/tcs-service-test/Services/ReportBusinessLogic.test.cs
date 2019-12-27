using AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using tcs_service.EF;
using tcs_service.Models.DTO;
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
        public void SuccessReport_StudentsSummedCorrectly()
        {
            var courseWithGradeList = new List<CourseWithGradeDTO>();
            
            for (int i = 0; i < 100; i++)
            {
                Random rand = new Random();
                var num = rand.Next(3);
                var courseWithGrade = fixture.Create<CourseWithGradeDTO>();
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
    }
}
