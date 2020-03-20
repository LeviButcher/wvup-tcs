using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tcs_service.EF;
using tcs_service.Models;
using tcs_service.Repos;
using tcs_service_test.Helpers;
using Xunit;

namespace tcs_service_test.Repos {
    public class ClasstourRepoTest : IDisposable {
        readonly ClassTourRepo classTourRepo;
        readonly string dbName = "ClasstourRepoTest";
        readonly TCSContext db;

        public ClasstourRepoTest () {
            var dbOptions = DbInMemory.getDbInMemoryOptions (dbName);
            db = new TCSContext (dbOptions);
            classTourRepo = new ClassTourRepo (dbOptions);
        }

        public void Dispose () {
            db.Database.EnsureDeleted ();
        }

        [Fact]
        public async void ClassTour_DeletedIsFalse_DeletedSetToTrueAfterCallingRemove () {
            var tour = new ClassTour () { DayVisited = DateTime.Now, Name = "Some High School", NumberOfStudents = 23, Deleted = false };

            await classTourRepo.Create (tour);

            var deletedTour = await classTourRepo.Remove (x => x.Id == tour.Id);

            Assert.True (deletedTour.Deleted);
        }

        [Fact]
        public async void ClassTour_DeletedIsFalse_TourIsReturnedByGetAll () {
            var tour = new ClassTour () { DayVisited = DateTime.Now, Name = "Some High School", NumberOfStudents = 23, Deleted = false };
            await classTourRepo.Create (tour);
            var results = classTourRepo.GetAll ();

            Assert.Equal (tour.DayVisited, results.FirstOrDefault ().DayVisited);
        }

        [Fact]
        public async void ClassTour_DeletedIsTrue_TourNotReturnedByGetAll () {
            var tour = new ClassTour () { DayVisited = DateTime.Now, Name = "Some High School", NumberOfStudents = 23, Deleted = true };
            await classTourRepo.Create (tour);
            var results = classTourRepo.GetAll ();

            Assert.Empty (results);
        }

        [Fact]
        public async void ClassTour_DeletedIsFalse_TourIsReturnedByOverloadedGetAll () {
            var tour = new ClassTour () { DayVisited = DateTime.Now, Name = "Some High School", NumberOfStudents = 23, Deleted = false };
            await classTourRepo.Create (tour);
            var results = classTourRepo.GetAll (x => x.Name == "Some High School");

            Assert.Equal (tour.DayVisited, results.FirstOrDefault ().DayVisited);
        }

        [Fact]
        public async void ClassTour_DeletedIsTrue_TourNotReturnedByOverloadedGetAll () {
            var tour = new ClassTour () { DayVisited = DateTime.Now, Name = "Some High School", NumberOfStudents = 23, Deleted = true };
            await classTourRepo.Create (tour);
            var results = classTourRepo.GetAll (x => x.Name.Equals ("Some High School"));

            Assert.Empty (results);
        }
    }
}