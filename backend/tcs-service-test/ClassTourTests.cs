using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections;
using System.Net.Http;
using System.Threading.Tasks;
using tcs_service.Controllers;
using tcs_service.Models;
using tcs_service.Repos.Interfaces;
using Xunit;

namespace tcs_service_test
{
    public class ClassTourTests
    {
        ClassTour classTour;
        ClassToursController sut;
        Mock<IClassTourRepo> repository;
        IFixture fixture = new Fixture()
           .Customize(new AutoMoqCustomization());

        public ClassTourTests()
        {       
            classTour = fixture.Create<ClassTour>();
            repository = new Mock<IClassTourRepo>();
            sut = new ClassToursController(repository.Object);
        }

        [Fact]
        public async void GetTourWithID_ShouldWork_Returns200()
        {
            repository.Setup(x => x.Add(classTour)).ReturnsAsync(classTour);

            IActionResult results = await sut.GetClassTour(classTour.ID);
            var okObjectResult = Assert.IsType<OkObjectResult>(results);
            
            Assert.Equal(classTour, okObjectResult.Value);
            Assert.Equal(200, okObjectResult.StatusCode);   
        }

        [Fact]
        public async void AddingNewTour_ShouldWork_Returns201()
        {
            repository.Setup(x => x.Find(classTour.ID)).ReturnsAsync(classTour);

            IActionResult results = await sut.PostClassTour(classTour);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(results);

            Assert.Equal(classTour, createdAtActionResult.Value);
            Assert.Equal(201, createdAtActionResult.StatusCode);
        }

        [Fact]
        public async void UpdatingTour_ShouldWork_Returns200()
        {
            var updatedClassTour = fixture.Build<ClassTour>()
                .With(x => x.ID, classTour.ID)
                .With(x => x.Name, "updatedName")
                .Create();

            repository.Setup(x => x.Update(updatedClassTour)).ReturnsAsync(updatedClassTour);


            IActionResult results = await sut.PutClassTour(classTour.ID, updatedClassTour);
            var okObjectResult = Assert.IsType<OkObjectResult>(results);

            Assert.Equal(updatedClassTour, okObjectResult.Value);
            Assert.Equal(200, okObjectResult.StatusCode);
        }

        [Fact]
        public async void UpdatingTourWithIDNotInDb_ShouldReturnBadRequest()
        {
            
            IActionResult results = await sut.PutClassTour(classTour.ID + 1, classTour);
            var badRequestResult = Assert.IsType<BadRequestResult>(results);

            Assert.Equal(400, badRequestResult.StatusCode);
        }


        // This test fails. Need change in controller to make sure name cannot be set to null
        [Fact]
        public async void UpdatingTourWithNameSetToNull_ShouldReturnBadRequest()
        {
            var nullNameClassTour = fixture.Build<ClassTour>()
                .With(x => x.ID, classTour.ID)
                .Without(x => x.Name)
                .Create();

            IActionResult results = await sut.PutClassTour(classTour.ID, nullNameClassTour);
            var badRequestResult = Assert.IsType<OkObjectResult>(results);

            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async void DeleteTour_ShouldWork_Returns200()
        {
            repository.Setup(x => x.Exist(classTour.ID)).ReturnsAsync(true);
            repository.Setup(x => x.Remove(classTour.ID)).ReturnsAsync(classTour);


            IActionResult results = await sut.DeleteClassTour(classTour.ID);
            var okObjectResult = Assert.IsType<OkObjectResult>(results);

            Assert.Equal(classTour, okObjectResult.Value);
            Assert.Equal(200, okObjectResult.StatusCode);
        }

        // Test fails because not mocking httpRequest yet
        [Fact]
        public async void GetAllTours_ShouldWork_Returns200andCount()
        { 

            var tours = fixture.CreateMany<ClassTour>();
            repository.Setup(x => x.GetAll()).Returns(tours);
            //Maybe need to make new httpClient
            //sut.Request = HttpClient();
            IActionResult results = sut.GetClassTour();
            var objectResult = Assert.IsType<ObjectResult>(results);

            Assert.Equal(tours, objectResult.Value);
            Assert.Equal(200, objectResult.StatusCode);
        }
    }
}
