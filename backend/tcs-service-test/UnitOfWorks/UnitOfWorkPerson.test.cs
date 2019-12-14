using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Options;
using Moq;
using tcs_service.Helpers;
using tcs_service.Models;
using tcs_service.Repos.Interfaces;
using tcs_service.Repos;
using tcs_service.UnitOfWorks;
using Xunit;
using System.Linq;
using System.Collections.Generic;
using tcs_service_test.Helpers;
using tcs_service.Services.Interfaces;
using tcs_service.EF;

namespace tcs_service_test.Controllers
{
    public class UnitOfWorkPersonTest : IDisposable
    {
        readonly string dbName = "UnitOfWorkPersonTest";
        readonly UnitOfWorkPerson unitPerson;
        readonly TCSContext db;

        readonly Mock<IBannerService> mockBannerService;

        public UnitOfWorkPersonTest()
        {
            var dbInMemory = DbInMemory.getDbInMemoryOptions(dbName);
            var personRepo = new PersonRepo(dbInMemory);
            var courseRepo = new CourseRepo(dbInMemory);
            var scheduleRepo = new ScheduleRepo(dbInMemory);
            var semesterRepo = new SemesterRepo(dbInMemory);
            db = new TCSContext(dbInMemory);
            mockBannerService = new Mock<IBannerService>();

            unitPerson = new UnitOfWorkPerson(personRepo, scheduleRepo, courseRepo, semesterRepo, mockBannerService.Object);
        }

        public void Dispose()
        {
            db.Database.EnsureDeleted();
        }


        // person should be saved
        // courses should be saved
        // schedule should be saved
        // semester should be saved
        [Fact]
        public async void GetPersonInfo_withEmail_StudentIsNotSavedInDatabase_ShouldReturnPersonInfoAndSaveToDatabase()
        {
            var email = "lbutche3@wvup.edu";
            var courses = new List<BannerCourse>()
            {
                new BannerCourse() {
                    CourseName="Intro to Computation",
                    CRN=13548,
                    ShortName="CS 101",
                    Department= new BannerDepartment(){
                        Name="STEM",
                        Code=42
                    }
                }
            };
            var semesterCode = 201902;
            var bannerPersonInfo = new BannerPersonInfo()
            {
                Id = 1,
                Email = email,
                Courses = courses,
                FirstName = "Bob",
                LastName = "Dylan",
                SemesterId = semesterCode
            };
            mockBannerService.Setup(x => x.GetBannerInfo(It.IsAny<string>())).ReturnsAsync(() => bannerPersonInfo);


            var personInfo = await unitPerson.GetPersonInfo(email, new DateTime(2019, 7, 1));


            Assert.NotNull(personInfo);
            var person = db.People.FirstOrDefault(x => x.Email == email);
            Assert.NotNull(person);
            var semester = db.Semesters.FirstOrDefault(x => x.Code == semesterCode);
            Assert.NotNull(semester);
            courses.ForEach(c =>
            {
                var course = db.Courses.FirstOrDefault(x => c.CRN == x.CRN);
                Assert.NotNull(course);
                var department = db.Departments.FirstOrDefault(x => x.Code == c.Department.Code);
                Assert.NotNull(department);
            });

            var schedules = db.Schedules.Where(x => x.PersonId == person.Id);
            Assert.Equal(schedules.Count(), courses.Count);
            Assert.All(schedules, x => Assert.Equal(x.PersonId, person.Id));
        }

        // Banner should be called
        // New Courses should be saved
        // New Schedule should be saved
        // New Semester should be saved
        [Fact]
        public async void GetPersonInfo_withEmail_StudentExistsButHasOldSchedule_ShouldReturnStudentInfoAndSaveToDatabase()
        {
            var person = new Person()
            {
                Email = "lbutche3@wvup.edu",
                FirstName = "Billy",
                LastName = "Joe",
                Id = 134657
            };
            var semester = new Semester()
            {
                Code = 201901
            };

            var newSemesterId = 201902;
            var oldCourses = new List<Course>() {
                new Course() {
                    CRN = 13465,
                    Name = "Intro to Computers",
                    Department = new Department() {
                        Code = 13465,
                        Name = "STEM"
                    }
                }
            };

            var bannerPersonInfo = new BannerPersonInfo()
            {
                Id = 1,
                Email = person.Email,
                Courses = new List<BannerCourse>() {
                    new BannerCourse(){
                    CRN = 11112,
                    CourseName = "Intro to Excel",
                    ShortName = "CS 101",
                    Department = new BannerDepartment() {
                        Code = 1234,
                        Name = "STEM"
                        }
                    }
                },
                FirstName = person.FirstName,
                LastName = person.LastName,
                SemesterId = newSemesterId
            };

            db.People.Add(person);
            db.Semesters.Add(semester);
            db.Courses.AddRange(oldCourses);
            db.SaveChanges();

            mockBannerService.Setup(x => x.GetBannerInfo(It.IsAny<string>())).ReturnsAsync(() => bannerPersonInfo);

            var personInfo = await unitPerson.GetPersonInfo(person.Email, new DateTime(2019, 8, 1));



            mockBannerService.Verify(x => x.GetBannerInfo(It.Is<string>(a => a == person.Email)), Times.Once());

            Assert.Equal(personInfo.Schedule.Select(x => x.CRN), bannerPersonInfo.Courses.Select(x => x.CRN));
            var addedSemester = db.Semesters.SingleOrDefault(x => x.Code == newSemesterId);
            Assert.NotNull(addedSemester);
            Assert.Equal(personInfo.Schedule.Select(x => x.CRN), db.Schedules.Where(x => x.Person.Email == person.Email).Select(x => x.CourseCRN));
        }


        [Fact]
        public async void GetPersonInfo_withEmail_StudentDoesNotExist_StudentCoursesDoExist_ShouldReturnStudentAndNotSaveNewCoursesToDatabase()
        {
            var person = new Person()
            {
                Email = "lbutche3@wvup.edu",
                FirstName = "Billy",
                LastName = "Careless",
                PersonType = PersonType.Student
            };
            var semester = new Semester()
            {
                Code = 201901
            };
            var courses = new List<Course>() {
                new Course() {
                    CRN = 16748,
                    Name = "Yoga",
                    ShortName = "GYM302",
                    Department = new Department() {
                        Code = 234,
                        Name = "Physical Wellness"
                    }
                }
            };
            db.Courses.AddRange(courses);
            db.SaveChanges();

            var bannerPersonInfo = new BannerPersonInfo()
            {
                Id = 1,
                Email = person.Email,
                Courses = courses.Select(x => new BannerCourse()
                {
                    CourseName = x.Name,
                    CRN = x.CRN,
                    ShortName = x.ShortName,
                    Department = new BannerDepartment()
                    {
                        Code = x.Department.Code,
                        Name = x.Department.Name
                    }
                }),
                FirstName = person.FirstName,
                LastName = person.LastName,
                SemesterId = semester.Code
            };

            mockBannerService.Setup(x => x.GetBannerInfo(It.IsAny<string>())).ReturnsAsync(() => bannerPersonInfo);

            var personInfo = await unitPerson.GetPersonInfo(person.Email, new DateTime(2019, 2, 1));

            mockBannerService.Verify(x => x.GetBannerInfo(It.Is<string>(a => a == person.Email)), Times.Once());

            var addedPerson = db.People.Find(personInfo.Id);
            Assert.NotNull(addedPerson);
            Assert.Equal(courses.Count(), db.Courses.Count());
            var schedule = db.Schedules.Where(x => x.PersonId == addedPerson.Id);
            Assert.Equal(schedule.Count(), courses.Count());
            Assert.NotNull(db.Semesters.Find(semester.Code));
        }

        [Fact]
        public async void GetPersonInfo_withEmail_TeacherDoesNotExists_ShouldReturnTeacherInfoAndSaveToDatabase()
        {
            var teacherEmail = "teacher@wvup.edu";
            var bannerPersonInfo = new BannerPersonInfo()
            {
                Email = teacherEmail,
                FirstName = "Barry",
                LastName = "Lomphson",
                Teacher = true
            };

            mockBannerService.Setup(x => x.GetBannerInfo(It.IsAny<string>())).ReturnsAsync(() => bannerPersonInfo);

            var personInfo = await unitPerson.GetPersonInfo(teacherEmail, new DateTime(2019, 2, 1));

            var teacher = db.People.LastOrDefault(x => x.Email == teacherEmail);
            Assert.NotNull(teacher);
            Assert.Equal(PersonType.Teacher, teacher.PersonType);
            var semester = db.Semesters.Last();
            Assert.NotNull(semester);
        }

        [Fact]
        public async void GetPersonInfo_withEmail_StudentExistsAndHasSemesterSchedule_ShouldReturnStudentInfoWithoutCallingBanner()
        {
            var person = new Person()
            {
                Email = "lbutche3@wvup.edu",
                FirstName = "Billy",
                LastName = "Bob",
                Id = 1635489
            };
            var semester = new Semester()
            {
                Code = 201901
            };
            var courses = new List<Course>(){
                new Course(){
                    CRN=163574,
                    Name="Intro to Powerpoint",
                    ShortName="PO124",
                    Department= new Department(){
                        Code=315,
                        Name="STEM"
                    }
                }
            };
            var schedule = new Schedule()
            {
                PersonId = person.Id,
                CourseCRN = courses[0].CRN,
                SemesterCode = semester.Code
            };
            db.Semesters.Add(semester);
            db.People.Add(person);
            db.Courses.AddRange(courses);
            db.Schedules.Add(schedule);
            await db.SaveChangesAsync();

            var bannerPersonInfo = new BannerPersonInfo()
            {
                Id = 1,
                Email = person.Email,
                Courses = null,
                FirstName = "Bob",
                LastName = "Dylan",
                SemesterId = semester.Code
            };
            mockBannerService.Setup(x => x.GetBannerInfo(It.IsAny<string>())).ReturnsAsync(() => bannerPersonInfo);


            var personInfo = await unitPerson.GetPersonInfo(person.Email, new DateTime(2019, 1, 1));

            Assert.NotNull(personInfo);
            Assert.Equal(personInfo.Email, person.Email);
            mockBannerService.Verify(x => x.GetBannerInfo(It.IsAny<string>()), Times.Never());
            Assert.Equal(courses.Select(x => x.CRN), personInfo.Schedule.ToList().Select(x => x.CRN));
        }

    }
}