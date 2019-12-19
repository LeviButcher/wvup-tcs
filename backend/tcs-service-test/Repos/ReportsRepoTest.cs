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
        readonly ReportsRepo reportsRepo;
        readonly string dbName = "ReportRepoTest";
        readonly IFixture fixture;
        readonly TCSContext db;


        public ReportsRepoTest()
        {
            var dbOptions = DbInMemory.getDbInMemoryOptions(dbName);
            db = new TCSContext(dbOptions);
            IBannerService bs = new MockBannerService(new PersonRepo(dbOptions), new SemesterRepo(dbOptions), new ClassRepo(dbOptions));
            reportsRepo = new ReportsRepo(db, bs);
            fixture = new Fixture()
              .Customize(new AutoMoqCustomization());
            // Setting default values
            fixture.RepeatCount = 3;
            fixture.Customize<Session>((ob) => ob.Without(x => x.SessionClasses).Without(x => x.Person)
                .Without(x => x.Semester).Without(x => x.SessionReasons));
        }

        public void Dispose()
        {
            db.Database.EnsureDeleted();
        }

        [Fact]
        public async void WeeklyVisits_HappyPath()
        {
            var startDate = new DateTime(2019, 8, 4);
            var endDate = startDate.AddDays(5);
            var sessions = fixture.CreateMany<Session>().Select(x =>
            {
                x.InTime = startDate;
                x.OutTime = startDate.AddDays(2);
                return x;
            });

            db.Sessions.AddRange(sessions);
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
        public async void WeeklyVisits_SignInsSpreadAcross3Weeks()
        {
            var startingDate = new DateTime(2019, 8, 4);

            var weeks = new[]{
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

            var sessions = weeks.Aggregate(new List<Session>(), (acc, curr) =>
            {
                var weeksSignIns = fixture.CreateMany<Session>().Select(x =>
                {
                    x.InTime = curr.startDate;
                    x.OutTime = curr.startDate.AddDays(1);
                    return x;
                });
                acc.AddRange(weeksSignIns);
                return acc;
            });

            db.Sessions.AddRange(sessions);
            db.SaveChanges();

            var results = await reportsRepo.WeeklyVisits(weeks[0].startDate, weeks.Last().endDate);
            var expectedResult = weeks.Select(x => new WeeklyVisitsViewModel(x.startDate, x.endDate)
            {
                Count = 3
            }).ToList();

            Assert.Equal(expectedResult, results);
        }

        [Fact]
        public async void PeakHours_SignInsSpreadAcross2Months()
        {
            var startDate = new DateTime(2019, 8, 1);
            var endDate = new DateTime(2019, 10, 1);

            TimeSpan timeSpan = endDate - startDate;
            var sessions = new List<Session>();

            for(int i = 0; i < 100; i++)
            {
                var randomTest = new Random();
                TimeSpan newSpan = new TimeSpan(0, randomTest.Next(0, (int)timeSpan.TotalMinutes), 0);
                DateTime newDate = startDate + newSpan;
                var session = fixture.Create<Session>();
                session.InTime = newDate;
                session.OutTime = newDate.AddHours(1);
                sessions.Add(session);
            }            

            db.Sessions.AddRange(sessions);
            db.SaveChanges();

            var twelvePM = new List<int>();
            foreach(var s in sessions)
            {
                if(s.InTime.Value.Hour == 12)
                {
                    twelvePM.Add(s.InTime.Value.Hour);
                }
            }

            var twoPM = new List<int>();
            foreach (var s in sessions)
            {
                if (s.InTime.Value.Hour == 14)
                {
                    twoPM.Add(s.InTime.Value.Hour);
                }
            }

            var fivePM = new List<int>();
            foreach (var s in sessions)
            {
                if (s.InTime.Value.Hour == 17)
                {
                    fivePM.Add(s.InTime.Value.Hour);
                }
            }

            var results = await reportsRepo.PeakHours(startDate, endDate);

            var twelvePMActualCount = results.Any(x => x.Hour == "12 P.M") ? results.Find(x => x.Hour == "12 P.M") : new PeakHoursViewModel(12, 0);
            var twoPMActualCount = results.Any(x => x.Hour == "2 P.M") ? results.Find(x => x.Hour == "2 P.M") : new PeakHoursViewModel(14, 0);
            var fivePMActualCount = results.Any(x => x.Hour == "5 P.M") ? results.Find(x => x.Hour == "5 P.M") : new PeakHoursViewModel(17, 0);

            Assert.Equal(twelvePM.Count(), twelvePMActualCount.Count);
            Assert.Equal(twoPM.Count(), twoPMActualCount.Count);
            Assert.Equal(fivePM.Count(), fivePMActualCount.Count);
        }
    }
}