using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Linq.Expressions;
using tcs_service.Controllers;
using tcs_service.Models;
using tcs_service.Repos.Interfaces;
using Xunit;

namespace tcs_service_test
{
    public class ClassTourControllerTest : IDisposable
    {
        ClassTour classTour;
        ClassToursController sut;
        Mock<IClassTourRepo> repository;
        IFixture fixture = new Fixture()
           .Customize(new AutoMoqCustomization());

        public ClassTourControllerTest()
        {
            classTour = fixture.Create<ClassTour>();
            repository = new Mock<IClassTourRepo>();
            sut = new ClassToursController(repository.Object);
        }

        [Fact]
        public async void GetTourWithID_ShouldWork_Returns200()
        {
            repository.Setup(x => x.Find(It.IsAny<Expression<Func<ClassTour, Boolean>>>())).ReturnsAsync(classTour);

            IActionResult results = await sut.GetClassTour(classTour.Id);
            var okObjectResult = Assert.IsType<OkObjectResult>(results);

            Assert.Equal(classTour, okObjectResult.Value);
            Assert.Equal(200, okObjectResult.StatusCode);
        }

        [Fact]
        public async void AddingNewTour_ShouldWork_Returns201()
        {
            repository.Setup(x => x.Find(a => a.Id == classTour.Id)).ReturnsAsync(classTour);

            IActionResult results = await sut.PostClassTour(classTour);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(results);

            Assert.Equal(classTour, createdAtActionResult.Value);
            Assert.Equal(201, createdAtActionResult.StatusCode);
        }

        [Fact]
        public async void UpdatingTour_ShouldWork_Returns200()
        {
            var updatedClassTour = fixture.Build<ClassTour>()
                .With(x => x.Id, classTour.Id)
                .With(x => x.Name, "updatedName")
                .Create();

            repository.Setup(x => x.Update(updatedClassTour)).ReturnsAsync(updatedClassTour);

            IActionResult results = await sut.PutClassTour(classTour.Id, updatedClassTour);
            var okObjectResult = Assert.IsType<OkObjectResult>(results);

            Assert.Equal(updatedClassTour, okObjectResult.Value);
            Assert.Equal(200, okObjectResult.StatusCode);
        }

        [Fact]
        public async void UpdatingTourWithIDNotInDb_ShouldReturnBadRequest()
        {
            IActionResult results = await sut.PutClassTour(classTour.Id + 1, classTour);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(results);

            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async void DeleteTour_ShouldWork_Returns200()
        {
            repository.Setup(x => x.Exist(It.IsAny<Expression<Func<ClassTour, Boolean>>>())).ReturnsAsync(true);
            repository.Setup(x => x.Remove(It.IsAny<Expression<Func<ClassTour, Boolean>>>())).ReturnsAsync(classTour);

            IActionResult results = await sut.DeleteClassTour(classTour.Id);
            var okObjectResult = Assert.IsType<OkObjectResult>(results);

            Assert.Equal(classTour, okObjectResult.Value);
            Assert.Equal(200, okObjectResult.StatusCode);
        }

        [Fact]
        public async void GetClassTourNotInDb_ShouldFail_ReturnsNotFound()
        {
            var classTour = fixture.Create<ClassTour>();

            IActionResult results = await sut.GetClassTour(classTour.Id);
            var notFoundResult = Assert.IsType<NotFoundResult>(results);

            Assert.Equal(404, notFoundResult.StatusCode);
        }

        public void Dispose()
        {
            repository = null;
            sut = null;
        }

        //TODO:  Test ModelState Validation
    }
}
