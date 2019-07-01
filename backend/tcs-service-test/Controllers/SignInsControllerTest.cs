using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tcs_service.Controllers;
using tcs_service.Models;
using tcs_service.Models.ViewModels;
using tcs_service.Repos.Interfaces;
using Xunit;

namespace tcs_service_test.Controllers
{
    public class SignInsControllerTest : IDisposable
    {
        SignInsController sut;
        Mock<ISignInRepo> signInRepo;
        IFixture fixture = new Fixture()
            .Customize(new AutoMoqCustomization());

        public SignInsControllerTest()
        {
            signInRepo = new Mock<ISignInRepo>();
            sut = new SignInsController(signInRepo.Object);
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        public void Dispose()
        {
            sut = null;
            signInRepo = null;
        }

        [Fact]
        public void GetAllSignIns_ShouldPass()
        {            
            var signIns = fixture.CreateMany<SignIn>();
            signInRepo.Setup(x => x.GetAll()).Returns(signIns);
            var res = sut.GetSignIn();

            var objectResult = Assert.IsType<ObjectResult>(res);
            Assert.Equal(objectResult.Value, signIns);
        }

        [Fact]
        public async void GetSignInByID_IdIsFound_ShouldPass()
        {
            var signIn = fixture.Create<SignIn>();
            signInRepo.Setup(x => x.Find(signIn.ID)).ReturnsAsync(signIn);
            var res = await sut.GetSignIn(signIn.ID);

            var objectResult = Assert.IsType<OkObjectResult>(res);
            Assert.Equal(objectResult.Value, signIn);
        }

        [Fact]
        public async void WhileTutoringIsTrue_NoClassesSelected_ShouldFail()
        {
            var signIn = fixture.Build<SignInViewModel>()
                .With(x => x.Tutoring, true)
                .Without(x => x.Courses)
                .Create();

            var res = await sut.PostSignIn(signIn);
            var objectResult = Assert.IsType<BadRequestObjectResult>(res);
            Assert.Equal(400, objectResult.StatusCode);
        }

        [Fact]
        public async void WhileTutoringIsFalse_NoReasonsSelected_ShouldFail()
        {
            var signIn = fixture.Build<SignInViewModel>()
                .With(x => x.Tutoring, false)
                .Without(x => x.Reasons)
                .Create();

            var res = await sut.PostSignIn(signIn);
            var objectResult = Assert.IsType<BadRequestObjectResult>(res);
            Assert.Equal(400, objectResult.StatusCode);
        }

        [Fact]
        public async void CreateSignIn_WithPreviouslyUnexistingCourseAndDepartment_ShouldPass()
        {
            var department = fixture.Create<Department>();
            var course = fixture.Build<Course>()
                .With(x => x.Department, department)
                .With(x => x.DepartmentID, department.Code)
                .Create();
            var vm = fixture.Build<SignInViewModel>()
                .With(x => x.SemesterId, 202001)
                .Create();

            vm.Courses.Clear();
            vm.Courses.Add(course);

            var res = await sut.PostSignIn(vm);
            var objectResult = Assert.IsType<CreatedAtActionResult>(res);
            Assert.Equal(201, objectResult.StatusCode);
        }   
    }
}
