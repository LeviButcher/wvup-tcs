﻿using System.Numerics;
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
using AutoMapper;
using tcs_service.Helpers;

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
            var config = new MapperConfiguration(opt => opt.AddProfile<AutoMapperProfile>());
            var mapper = config.CreateMapper();
            sut = new SignInsController(signInRepo.Object, mapper);
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

            var objectResult = Assert.IsType<OkObjectResult>(res);
            Assert.Equal(objectResult.Value, signIns);
        }

        [Fact]
        public async void GetSignInByID_IdIsFound_ShouldPass()
        {
            var signIn = fixture.Create<SignInViewModel>();
            signInRepo.Setup(x => x.GetSignInViewModel(It.IsAny<int>())).ReturnsAsync(signIn);
            var res = await sut.GetSignIn(signIn.Id);

            var objectResult = Assert.IsType<OkObjectResult>(res);
            Assert.Equal(objectResult.Value, signIn);
        }

        [Fact]
        public async void WhileTeacherIsNotTrue_NoClassesSelected_ShouldFail()
        {
            var signIn = fixture.Build<SignInViewModel>()
                .With(x => x.Tutoring, true)
                .Without(x => x.Courses)
                .Create();

            await Assert.ThrowsAsync<TCSException>(async () =>
            {
                await sut.PostSignIn(signIn, false);
            });
        }

        [Fact]
        public async void WhileTutoringIsFalse_NoReasonsSelected_ShouldFail()
        {
            var signIn = fixture.Build<SignInViewModel>()
                .With(x => x.Tutoring, false)
                .Without(x => x.Reasons)
                .Create();
            await Assert.ThrowsAsync<TCSException>(async () =>
            {
                await sut.PostSignIn(signIn, false);
            });
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

            vm.Courses.Add(course);

            var res = await sut.PostSignIn(vm, false);
            var objectResult = Assert.IsType<CreatedResult>(res);
            Assert.Equal(201, objectResult.StatusCode);
        }
    }
}
