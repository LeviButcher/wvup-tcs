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
    public class LookupRepoTest : IDisposable
    {
        LookupRepo lookupRepo;
        string dbName = "LookupRepoTest";
        IFixture fixture;
        TCSContext db;


        public LookupRepoTest()
        {
            var dbOptions = DbInMemory.getDbInMemoryOptions(dbName);
            db = new TCSContext(dbOptions);
            lookupRepo = new LookupRepo(dbOptions);
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
        public async void GetBySemester_HappyPath()
        {
            var semester = fixture.Create<Semester>();
            var person = fixture.Create<Person>();
            var signIns = fixture.CreateMany<SignIn>();
            db.Semesters.Add(semester);
            db.People.Add(person);
            db.SaveChanges();
            signIns = signIns.Select(x =>
            {
                x.SemesterId = semester.Id;
                x.PersonId = person.Id;
                return x;
            });
            db.SignIns.AddRange(signIns);
            db.SaveChanges();
            var semesterSignins = await lookupRepo.GetBySemester(semester.Id);
            Assert.Equal(signIns.Count(), semesterSignins.Count());
        }
    }
}