using System;
using Moq;
using tcs_service.Models;
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
            var departmentRepo = new DepartmentRepo(dbInMemory);
            db = new TCSContext(dbInMemory);
            mockBannerService = new Mock<IBannerService>();

            unitPerson = new UnitOfWorkPerson(personRepo, scheduleRepo, courseRepo, semesterRepo, departmentRepo, mockBannerService.Object);
        }

        public void Dispose()
        {
            db.Database.EnsureDeleted();
        }

        private int ParseOrDefault(string toParse, int fallback)
        {
            int resultStorage;
            bool success = Int32.TryParse(toParse, out resultStorage);
            if (success) return resultStorage;
            else return fallback;
        }



        /*
            Use Case:
                The very first student goes to sign into the system. Nothing exists in the database for that student or their courses.
                Everything needs to be saved and banner needs to be called

            Assertions:
                BannerApi should be called once
                A new Student should be saved into Person table
                A new semester should be saved into Semester table
                That students courses should be saved into Courses table
                That students schedule should be saved into the Schedule table
        */
        [Theory]
        [InlineData("lbutche3@wvup.edu")]
        [InlineData("1345679321")]
        public async void GetPersonInfo_StudentIsNotSavedInDatabase_ShouldReturnPersonInfoAndSaveToDatabase(string identifier)
        {
            var email = identifier;
            var id = ParseOrDefault(identifier, 19834567);
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
                },
                new BannerCourse() {
                    CourseName="Intro to Web Design",
                    CRN=13464,
                    ShortName="CS 129",
                    Department = new BannerDepartment() {
                        Name="STEM",
                        Code=42
                    }
                }
            };
            var semesterCode = 201902;
            var bannerPersonInfo = new BannerPersonInfo()
            {
                Id = id,
                Email = identifier,
                Courses = courses,
                FirstName = "Bob",
                LastName = "Dylan",
                SemesterId = semesterCode
            };
            mockBannerService.Setup(x => x.GetBannerInfo(It.IsAny<string>())).ReturnsAsync(() => bannerPersonInfo);


            var personInfo = await unitPerson.GetPersonInfo(email, new DateTime(2019, 7, 1));

            mockBannerService.Verify(x => x.GetBannerInfo(It.Is<string>(a => a == bannerPersonInfo.Email || a == bannerPersonInfo.Id.ToString())), Times.Once());
            Assert.NotNull(personInfo);
            var person = db.People.FirstOrDefault(x => x.Email == email || x.Id == id);
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

        /*
            Use Case:
                A student had signed in the previous semester, but they are now signing in durning the next semester. This student is the first student to sign in this semester.

            Assertions:
                Banner api should be called once
                Student's new courses should be saved into Course table
                Student's new schedule should be saved into Semester table
                A new semester should be saved into Semester table
        */
        [Theory]
        [InlineData("rsaunch4@wvup.edu")]
        [InlineData("1354678945")]
        public async void GetPersonInfo_StudentExistsButHasOldSchedule_ShouldReturnStudentInfoAndSaveToDatabase(string identifier)
        {
            var email = identifier;
            var id = ParseOrDefault(identifier, 54321687);
            var person = new Person()
            {
                Id = id,
                Email = email,
                FirstName = "Billy",
                LastName = "Joe"
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
                Id = person.Id,
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

            var personInfo = await unitPerson.GetPersonInfo(identifier, new DateTime(2019, 8, 1));

            mockBannerService.Verify(x => x.GetBannerInfo(It.Is<string>(a => a == person.Email || a == person.Id.ToString())), Times.Once());

            Assert.Equal(personInfo.Schedule.Select(x => x.CRN), bannerPersonInfo.Courses.Select(x => x.CRN));
            var addedSemester = db.Semesters.SingleOrDefault(x => x.Code == newSemesterId);
            Assert.NotNull(addedSemester);
            Assert.Equal(personInfo.Schedule.Select(x => x.CRN), db.Schedules.Where(x => x.Person.Email == person.Email).Select(x => x.CourseCRN));
        }

        /*
            Use Case:
                A student has not signed in yet during this semester. Several student have signed in so that student's courses already exist in the system. The semester also already exists in the system.

            Assertions:
                BannerApi should be called once
                A new student should be saved
                That student's schedule should be saved
                No new courses should be saved in the database
                No new semesters should be saved in the database
        */
        [Theory]
        [InlineData("ssouth412@wvup.edu")]
        [InlineData("345676891")]
        public async void GetPersonInfo_StudentDoesNotExist_StudentCoursesDoExist_ShouldReturnStudentAndNotSaveNewCoursesToDatabase(string identifier)
        {
            var email = identifier;
            var id = ParseOrDefault(identifier, 54321687);
            var person = new Person()
            {
                Email = email,
                FirstName = "Billy",
                LastName = "Careless",
                Id = id,
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
            db.Semesters.Add(semester);
            db.SaveChanges();

            var bannerPersonInfo = new BannerPersonInfo()
            {
                Id = person.Id,
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

            var personInfo = await unitPerson.GetPersonInfo(identifier, new DateTime(2019, 2, 1));

            mockBannerService.Verify(x => x.GetBannerInfo(It.Is<string>(a => a == person.Email || a == person.Id.ToString())), Times.Once());

            var addedPerson = db.People.Find(personInfo.Id);
            Assert.NotNull(addedPerson);
            Assert.Equal(courses.Count(), db.Courses.Count());
            var schedule = db.Schedules.Where(x => x.PersonId == addedPerson.Id);
            Assert.Equal(schedule.Count(), courses.Count());
            Assert.NotNull(db.Semesters.Find(semester.Code));
        }

        /*
            Use Case:
                A teacher is trying to sign into the system. This teacher has never signed into the system before.

            Assertions:
                BannerApi should be called once
                A new teacher should be saved into the database

        */
        [Theory]
        [InlineData("teacher@wvup.edu")]
        [InlineData("149679543")]
        public async void GetPersonInfo_TeacherDoesNotExists_ShouldReturnTeacherInfoAndSaveToDatabase(string identifier)
        {
            var email = identifier;
            var id = ParseOrDefault(identifier, 54321687);
            var bannerPersonInfo = new BannerPersonInfo()
            {
                Email = email,
                Id = id,
                FirstName = "Barry",
                LastName = "Lomphson",
                Teacher = true
            };

            mockBannerService.Setup(x => x.GetBannerInfo(It.IsAny<string>())).ReturnsAsync(() => bannerPersonInfo);

            var personInfo = await unitPerson.GetPersonInfo(identifier, new DateTime(2019, 2, 1));

            var teacher = db.People.LastOrDefault(x => x.Email == bannerPersonInfo.Email || x.Id == bannerPersonInfo.Id);
            Assert.NotNull(teacher);
            Assert.Equal(PersonType.Teacher, teacher.PersonType);
            var semester = db.Semesters.Last();
            Assert.NotNull(semester);
        }


        /*
            Use Case:
                A student has frequently come to the tutoring center during a semester. They try to sign into the center for the 100th time this semester. Yay! The system should already have all their information on hand from their previous visits this semester.

            Assertions:
                BannerApi should not be called
                GetPersonInfo should return back this semester schedule
                The student shouldn't gain any more schedule records in the database
        */
        [Theory]
        [InlineData("lbutche3@wvup.edu")]
        [InlineData("315496794")]
        public async void GetPersonInfo_StudentExistsAndHasSemesterSchedule_ShouldReturnStudentInfoWithoutCallingBanner(string identifier)
        {
            var email = identifier;
            var id = ParseOrDefault(identifier, 54321687);
            var person = new Person()
            {
                Email = email,
                FirstName = "Billy",
                LastName = "Bob",
                Id = id
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
                Id = person.Id,
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
            var inDBSchedule = db.Schedules.Where(x => x.PersonId == person.Id);
            Assert.Equal(courses.Count(), inDBSchedule.Count());
        }
    }
}