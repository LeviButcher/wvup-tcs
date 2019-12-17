using AutoFixture;
using AutoFixture.AutoMoq;
using System;
using tcs_service.EF;
using System.Linq;
using tcs_service.Models;
using tcs_service.Repos;
using tcs_service_test.Helpers;
using Xunit;

namespace tcs_service_test.Repos
{
    public class SignInRepoTest : IDisposable
    {
        SignInRepo signInRepo;

        string dbName = "SignInRepoTest";

        IFixture fixture;

        TCSContext db;

        public SignInRepoTest()
        {

            var dbOptions = DbInMemory.getDbInMemoryOptions(dbName);
            db = new TCSContext(dbOptions);
            signInRepo = new SignInRepo(DbInMemory.getDbInMemoryOptions(dbName), null);
            fixture = new Fixture()
              .Customize(new AutoMoqCustomization());
            fixture.Customize<SignIn>((ob) => ob.With(x => x.InTime).With(x => x.OutTime).Without(x => x.ID).Without(x => x.Courses).Without(x => x.Reasons).Without(x => x.Person));
            fixture.Customize<SignInCourse>((ob) => ob.Without(x => x.SignInID).Without(x => x.SignIn));
            fixture.Customize<SignInReason>((ob) => ob.Without(x => x.SignInID).Without(x => x.SignIn));
            fixture.Customize<Course>((ob) => ob.Without(x => x.Department).Without(x => x.SignInCourses));
            fixture.Customize<Reason>((ob) => ob.Without(x => x.SessionReasons));
        }

        public void Dispose()
        {
            db.Database.EnsureDeleted();
        }

        [Fact]
        public async void CreateReason_ShouldWork()
        {
            var reason = fixture.Create<Reason>();

            var res = await signInRepo.AddReason(reason);

            Assert.Equal(reason.Name, res.Name);
        }

        [Fact]
        public async void ReasonExist_ShouldWork()
        {
            var reason = fixture.Create<Reason>();

            var obj = await signInRepo.AddReason(reason);

            var res = await signInRepo.ReasonExist(obj.Id);

            Assert.True(res);
        }

       
        [Fact]
        public async void AddCourse_SignInWithNewPersonNewCoursesAndReasons_ShouldWork()
        {
            var signIn = fixture.Create<SignIn>();
            signIn.InTime = DateTime.Now.AddHours(-1);
            signIn.OutTime = DateTime.Now;
            var courses = fixture.CreateMany<Course>().Select(x => new SignInCourse() { Course = x });
            var reasons = fixture.CreateMany<Reason>().Select(x => new SignInReason() { Reason = x });
            var semester = fixture.Create<Semester>();

            signIn.Courses.AddRange(courses);
            signIn.Reasons.AddRange(reasons);
            signIn.Semester = semester;

            var res = await signInRepo.Add(signIn);

            Assert.Equal(signIn.ID, res.ID);
            Assert.Equal(db.Courses.Count(), signIn.Courses.Count());
            Assert.Equal(db.Reasons.Count(), signIn.Reasons.Count());
        }

        // I was getting this to work with create new object or not all at once
        [Fact]
        public async void AddCourse_SignInWithExistingPersonAndNewAndExistingCoursesAndReasons_ShouldWork()
        {
            var person = fixture.Create<Person>();
            db.People.Add(person);
            var course = fixture.Create<Course>();
            db.Courses.Add(course);
            var reason = fixture.Create<Reason>();
            db.Reasons.Add(reason);
            await db.SaveChangesAsync();

            var signIn = fixture.Create<SignIn>();
            signIn.PersonId = person.Id;
            signIn.InTime = DateTime.Now.AddHours(-1);
            signIn.OutTime = DateTime.Now;
            var choosenCourses = fixture.CreateMany<Course>().Append(course).Select(x => new SignInCourse() { Course = x });
            var choosenReasons = fixture.CreateMany<Reason>().Append(reason).Select(x => new SignInReason() { Reason = x });
            signIn.Reasons.AddRange(choosenReasons);
            signIn.Courses.AddRange(choosenCourses);

            var res = await signInRepo.Add(signIn);

            Assert.Equal(signIn.ID, res.ID);
            Assert.Equal(db.Courses.Count(), signIn.Courses.Count());
            Assert.Equal(db.Reasons.Count(), signIn.Reasons.Count());
        }

        [Fact]
        public async void Update_HappyPath()
        {
            var signIn = fixture.Create<SignIn>();
            signIn.InTime = DateTime.Now.AddHours(-1);
            signIn.OutTime = DateTime.Now;
            // Technically add should enforce that a signIn have courses and reasons attached
            await signInRepo.Add(signIn);
            var result = await signInRepo.Update(signIn);
            Assert.Equal(signIn.OutTime, result.OutTime);
            Assert.Equal(signIn.ID, result.ID);
        }

        [Fact]
        public async void Update_WithAllNewCoursesAndReasons_ShouldSucceed()
        {
            var signIn = fixture.Create<SignIn>();
            signIn.InTime = DateTime.Now.AddHours(-1);
            signIn.OutTime = DateTime.Now;
            await signInRepo.Add(signIn);
            var courses = fixture.CreateMany<SignInCourse>().ToList();
            var reasons = fixture.CreateMany<SignInReason>().ToList();
            signIn.Courses = courses;
            signIn.Reasons = reasons;
            var result = await signInRepo.Update(signIn);

            Assert.Equal(result.Courses.Select(x => x.CourseID), courses.Select(x => x.CourseID));
            Assert.Equal(result.Reasons.Select(x => x.ReasonID), reasons.Select(x => x.ReasonID));
        }

        [Fact]
        public async void Update_WithRemovedCoursesAndReasons_ShouldSucceed()
        {
            var signIn = fixture.Create<SignIn>();
            signIn.InTime = DateTime.Now.AddHours(-1);
            signIn.OutTime = DateTime.Now;
            var courses = fixture.CreateMany<Course>().ToList();
            var reasons = fixture.CreateMany<Reason>().ToList();
            db.Courses.AddRange(courses);
            db.Reasons.AddRange(reasons);
            await db.SaveChangesAsync();

            signIn.Courses = courses.Select(c => new SignInCourse() { Course = c }).ToList();
            signIn.Reasons = reasons.Select(r => new SignInReason() { Reason = r }).ToList();
            await signInRepo.Add(signIn);

            var newCourses = signIn.Courses.Take(signIn.Courses.Count - 1);
            signIn.Courses = newCourses.ToList();
            var newReasons = signIn.Reasons.Take(signIn.Reasons.Count - 1);
            signIn.Reasons = newReasons.ToList();
            var result = await signInRepo.Update(signIn);

            Assert.Equal(newCourses.Select(x => x.CourseID), result.Courses.Select(x => x.CourseID));
            Assert.Equal(newReasons.Select(x => x.ReasonID), result.Reasons.Select(x => x.ReasonID));
        }

        [Fact]
        public async void Update_RemoveCourseAndReasonAddCourseAndReason_ShouldSucceed()
        {
            var signIn = fixture.Create<SignIn>();
            signIn.InTime = DateTime.Now.AddHours(-1);
            signIn.OutTime = DateTime.Now;
            var courses = fixture.CreateMany<Course>().ToList();
            var reasons = fixture.CreateMany<Reason>().ToList();
            db.Courses.AddRange(courses);
            db.Reasons.AddRange(reasons);
            await db.SaveChangesAsync();

            signIn.Courses = courses.Select(c => new SignInCourse() { Course = c }).ToList();
            signIn.Reasons = reasons.Select(r => new SignInReason() { Reason = r }).ToList();
            await signInRepo.Add(signIn);

            var newCourse = fixture.Create<Course>();
            var newReason = fixture.Create<Reason>();
            db.Courses.Add(newCourse);
            db.Reasons.Add(newReason);
            await db.SaveChangesAsync();

            var choosenCourses = signIn.Courses.Take(signIn.Courses.Count - 1).Append(new SignInCourse() { Course = newCourse, CourseID = newCourse.CRN });
            var choosenReasons = signIn.Reasons.Take(signIn.Reasons.Count - 1).Append(new SignInReason() { Reason = newReason, ReasonID = newReason.Id });
            signIn.Reasons = choosenReasons.ToList();
            signIn.Courses = choosenCourses.ToList();
            var result = await signInRepo.Update(signIn);

            Assert.Equal(choosenCourses.Select(x => x.CourseID), result.Courses.Select(x => x.CourseID));
            Assert.Equal(choosenReasons.Select(X => X.ReasonID), result.Reasons.Select(x => x.ReasonID));
        }
    }
}
