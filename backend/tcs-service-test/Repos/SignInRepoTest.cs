using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Options;
using Moq;
using System;
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

        readonly string secret = "supersecretpassphrasethatissuperlongbecauseithastobe";

        IFixture fixture;

        Mock<IOptions<AppSettings>> mockAppSettings;

        public SignInRepoTest()
        {
            AppSettings appSettings = new AppSettings() { Secret = secret };
            mockAppSettings = new Mock<IOptions<AppSettings>>();
            mockAppSettings.Setup(ap => ap.Value).Returns(appSettings);
            signInRepo = new SignInRepo(DbInMemory.getDbInMemoryOptions(dbName), null);
            fixture = new Fixture()
              .Customize(new AutoMoqCustomization());
        }

        public void Dispose()
        {
            signInRepo = null;
            fixture = null;
        }

        [Fact]
        public async void CreateSignIn_PreviouslyNonExistingStudentAndClass_ShouldPass()
        {
            var reason = fixture.Create<Reason>();

            var res = await signInRepo.AddReason(reason);

            Assert.Equal(reason.Name, res.Name);
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
    }
}
