using System;
using Xunit;
using tcs_service.Repos;
using tcs_service.EF;
using tcs_service_test.Helpers;
using System.Linq;
using AutoFixture;
using System.Collections.Generic;
using tcs_service.Models.ViewModels;
using tcs_service.Models;
using AutoFixture.AutoMoq;
using tcs_service.Services;
using tcs_service.Services.Interfaces;

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
            IBannerService bs = new MockBannerService(dbOptions);
            reportsRepo = new ReportsRepo(dbOptions, bs);
            fixture = new Fixture()
              .Customize(new AutoMoqCustomization());
            // Setting default values
            fixture.RepeatCount = 3;
            fixture.Customize<SignIn>((ob) => ob.Without(x => x.Courses).Without(x => x.Person)
                .Without(x => x.Semester).Without(x => x.Reasons));
        }

        public void Dispose()
        {
            db.Database.EnsureDeleted();
        }

        [Fact]
        public async void WeeklyVisits_HappyPath() {
            var startDate = new DateTime(2019, 8, 4);
            var endDate = startDate.AddDays(5);
            var signins = fixture.CreateMany<SignIn>().Select(x => {
                    x.InTime = startDate;
                    x.OutTime = startDate.AddDays(2);
                    return x;
                });

            db.SignIns.AddRange(signins);
            db.SaveChanges();
            
            var results = await reportsRepo.WeeklyVisits(startDate, endDate);
            var expectedResult = new List<WeeklyVisitsViewModel>(){
                new WeeklyVisitsViewModel(startDate, startDate.AddDays(6)){
                    Count = 3
                }
            };
            
            Assert.Equal(results[0].Count, expectedResult[0].Count);
            Assert.Equal(results[0].Item, expectedResult[0].Item);
        }

        [Fact]
        public async void WeeklyVisits_SignInsSpreadAcross3Weeks() {
            var startingDate = new DateTime(2019, 8, 4);

            var weeks = new []{
                new {
                    startDate = startingDate,
                    endDate = startingDate.AddDays(6)
                },
                new {
                    startDate = startingDate.AddDays(7),
                    endDate = startingDate.AddDays(7 + 6)
                },
                new {
                    startDate = startingDate.AddDays(14),
                    endDate = startingDate.AddDays(14 + 6)
                }
            }.ToList();

            var signIns = weeks.Aggregate(new List<SignIn>(), (acc, curr) => {
                var weeksSignIns = fixture.CreateMany<SignIn>().Select(x => {
                    x.InTime = curr.startDate;
                    x.OutTime = curr.startDate.AddDays(1);
                    return x;
                });
                acc.AddRange(weeksSignIns);
                return acc;
            });

            db.SignIns.AddRange(signIns);
            db.SaveChanges();
            
            var results = await reportsRepo.WeeklyVisits(weeks[0].startDate, weeks.Last().endDate);
            var expectedResult = weeks.Select(x => new WeeklyVisitsViewModel(x.startDate, x.endDate)
            {
                Count = 3
            }).ToList();
            
            Assert.Equal(expectedResult, results);
        }
    }
}