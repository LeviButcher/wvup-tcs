using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Options;
using Moq;
using System;
using tcs_service.EF;
using System.Linq;
using tcs_service.Helpers;
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
            fixture.Customize<SignIn>((ob) => ob.Without(x => x.ID).Without(x => x.Courses).Without(x => x.Reasons).Without(x => x.Person));
            fixture.Customize<SignInCourse>((ob) => ob.Without(x => x.SignInID).Without(x => x.SignIn));
            fixture.Customize<SignInReason>((ob) => ob.Without(x => x.SignInID).Without(x => x.SignIn));
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

            var res = await signInRepo.ReasonExist(obj.ID);

            Assert.True(res);
        }

        [Fact]
        public async void AddCourseWithDepartment_ShouldWork()
        {
            var dept = fixture.Create<Department>();

            var course = fixture.Build<Course>()
                .With(x => x.Department, dept)
                .Create();

            var res = await signInRepo.AddCourse(course);

            Assert.True(await signInRepo.DepartmentExist(dept.Code));
            Assert.Equal(res.CRN, course.CRN);
        }

        [Fact]
        public async void AddCourseAndDepartment_ShouldWork()
        {
            var dept = fixture.Create<Department>();

            var department = await signInRepo.AddDepartment(dept);

            var course = fixture.Build<Course>()
                .With(x => x.Department, department)
                .Create();

            var res = await signInRepo.AddCourse(course);

            Assert.True(await signInRepo.DepartmentExist(department.Code));
            Assert.Equal(res.CRN, course.CRN);
        }

        [Fact]
        public async void Update_HappyPath()
        {
            var signIn = fixture.Create<SignIn>();
            // Technically add should enforce that a signIn have courses and reasons attached
            await signInRepo.Add(signIn);
            signIn.OutTime = new DateTime();
            var result = await signInRepo.Update(signIn);
            Assert.Equal(signIn.OutTime, result.OutTime);
            Assert.Equal(signIn.ID, result.ID);
        }

        [Fact]
        public async void Update_WithAllNewCoursesAndReasons_ShouldSucceed()
        {
            var signIn = fixture.Create<SignIn>();
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
            var courses = fixture.CreateMany<SignInCourse>().ToList();
            var reasons = fixture.CreateMany<SignInReason>().ToList();
            signIn.Courses = courses;
            signIn.Reasons = reasons;
            await signInRepo.Add(signIn);

            var newCourses = signIn.Courses.Take(signIn.Courses.Count - 1);
            signIn.Courses = newCourses.ToList();
            var newReasons = signIn.Reasons.Take(signIn.Reasons.Count - 1);
            signIn.Reasons = newReasons.ToList();
            var result = await signInRepo.Update(signIn);

            Assert.Equal(newCourses, result.Courses);
            Assert.Equal(newReasons, result.Reasons);
        }

        [Fact]
        public async void Update_RemoveCourseAndReasonAddCourseAndReason_ShouldSucceed()
        {
            var signIn = fixture.Create<SignIn>();
            var courses = fixture.CreateMany<SignInCourse>().ToList();
            var reasons = fixture.CreateMany<SignInReason>().ToList();
            signIn.Courses = courses;
            signIn.Reasons = reasons;
            await signInRepo.Add(signIn);

            var newCourses = signIn.Courses.Take(signIn.Courses.Count - 1);
            newCourses = newCourses.Append(fixture.Create<SignInCourse>());
            signIn.Courses = newCourses.ToList();
            var newReasons = signIn.Reasons.Take(signIn.Reasons.Count - 1);
            newReasons = newReasons.Append(fixture.Create<SignInReason>());
            signIn.Reasons = newReasons.ToList();
            var result = await signInRepo.Update(signIn);

            Assert.Equal(newCourses.Select(x => x.CourseID), result.Courses.Select(x => x.CourseID));
            Assert.Equal(newReasons.Select(X => X.ReasonID), result.Reasons.Select(x => x.ReasonID));
        }
    }
}
