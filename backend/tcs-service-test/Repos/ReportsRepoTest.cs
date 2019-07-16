using System;
using Xunit;
using tcs_service.Repos;
using tcs_service.EF;
using tcs_service_test.Helpers;
using AutoFixture;
using System.Collections.Generic;
using tcs_service.Models.ViewModels;
using tcs_service.Models;
using AutoFixture.AutoMoq;

namespace tcs_service_test.Repos
{
    public class ReportsRepoTest : IDisposable
    {
        ReportsRepo reportsRepo;
        string dbName = "ReportRepoTest";
        IFixture fixture;
        TCSContext db;

        public ReportsRepoTest() {
            var dbOptions = DbInMemory.getDbInMemoryOptions(dbName);
            db = new TCSContext(dbOptions);
            reportsRepo = new ReportsRepo(dbOptions);
            fixture = new Fixture()
              .Customize(new AutoMoqCustomization());
            // Setting default values
            fixture.Customize<SignIn>((ob) => ob.Without(x => x.Courses).Without(x => x.Person).Without(x => x.Semester));
        }

        public void Dispose()
        {
            db.Database.EnsureDeleted();
        }

        [Fact]
        public async void WeeklyVisits_HappyPath() {
            var signins = fixture.Build<SignIn>().With(x => x.InTime, new DateTime()).With(x => x.OutTime, new DateTime()).CreateMany();
            db.SignIns.AddRange(signins);

            var startDate = new DateTime();
            var endDate = startDate.AddDays(5);
            var results = await reportsRepo.WeeklyVisits(startDate, endDate);
            var expectedResult = new List<ReportCountViewModel>(){
                new ReportCountViewModel{
                    Item = 5,
                    Count = 3
                }
            };
            
            Assert.Equal(results, expectedResult);
        }
    }
}