using System;
using System.Collections.Generic;
using System.Text;
using tcs_service.EF;
using tcs_service.Models;
using tcs_service.Repos;
using tcs_service_test.Helpers;
using Xunit;

namespace tcs_service_test.Repos {
    public class SemesterRepoTest : IDisposable {
        readonly SemesterRepo semestersRepo;
        readonly string dbName = "SemesterRepoTest";
        readonly TCSContext db;

        public SemesterRepoTest () {
            var dbOptions = DbInMemory.getDbInMemoryOptions (dbName);
            db = new TCSContext (dbOptions);
            semestersRepo = new SemesterRepo (dbOptions);
        }

        public void Dispose () {
            db.Database.EnsureDeleted ();
        }

        [Fact]
        public async void SemesterCodeEndsIn01_NameShouldBeFallAndYear () {
            var semester = new Semester () { Code = 202201 };

            var savedSemester = await semestersRepo.Create (semester);

            Assert.Equal ("Fall 2021", savedSemester.Name);
        }

        [Fact]
        public async void SemesterCodeEndsIn02_NameShouldBeSpringAndYear () {
            var semester = new Semester () { Code = 202202 };

            var savedSemester = await semestersRepo.Create (semester);

            Assert.Equal ("Spring 2022", savedSemester.Name);
        }

        [Fact]
        public async void SemesterCodeEndsIn03_NameShouldBeSummerAndYear () {
            var semester = new Semester () { Code = 202203 };

            var savedSemester = await semestersRepo.Create (semester);

            Assert.Equal ("Summer 2022", savedSemester.Name);
        }
    }
}