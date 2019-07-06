using System.Reflection;
using System.Linq;
using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using tcs_service.Controllers;
using tcs_service.Helpers;
using tcs_service.Models;
using tcs_service.Repos.Interfaces;
using Xunit;
using System.Collections.Generic;
using tcs_service.Exceptions;

namespace tcs_service_test.Controllers {
    public class UsersControllerTest : IDisposable {

        // Seriously, it has to be long
        String secret = "AsuperlongkeyisttoredhereIguessbecuaseitbreaksotherwise";
        UsersController usersController;
        Mock<IUserRepo> userRepo;
        IFixture fixture = new Fixture ()
            .Customize (new AutoMoqCustomization ());

        Mock<IOptions<AppSettings>> mockAppSettings;

        public UsersControllerTest () {
            userRepo = new Mock<IUserRepo> ();
            // Manually creating the methods AutoMapper will support
            var config = new MapperConfiguration(opts => {
                opts.CreateMap<UserDto, User>();
                opts.CreateMap<User, UserDto>();
            });
            var mapper = config.CreateMapper();
            // Creating a Fake AppSettings for Use in Signing JWT
            AppSettings appSettings = new AppSettings(){Secret=secret};
            mockAppSettings = new Mock<IOptions<AppSettings>> ();
            mockAppSettings.Setup(ap => ap.Value).Returns(appSettings);
            usersController = new UsersController (userRepo.Object, mapper, mockAppSettings.Object);
        }

        public void Dispose () {
            userRepo = null;
            mockAppSettings = null;
            usersController = null;
        }

        [Fact]
        public void GetAll_HappyPath() {
            var users = fixture.CreateMany<User>();
            userRepo.Setup(x => x.GetAll()).Returns(users);
            var res = usersController.GetAll();

            var objectResult = Assert.IsType<OkObjectResult>(res);
            List<UserDto> getUserRes = (List<UserDto>) objectResult.Value;
            Assert.Equal(getUserRes.Count(), users.Count());
        }

        [Fact]
        public async void Register_HappyPath() {
            userRepo.Setup(x => x.Create(It.IsAny<User>(), It.IsAny<String>())).ReturnsAsync((User u, String password) => u);
            var user = fixture.Create<UserDto>();
            
            var res = await usersController.Register(user);
            
            Assert.IsType<CreatedResult>(res);
        }

        [Fact]
        public async void Register_ExceptionThrown_ReturnsBadRequest() {
            var message = "Password Not Provided";
            userRepo.Setup(x => x.Create(It.IsAny<User>(), It.IsAny<String>())).ThrowsAsync(new AppException(message));
            var user = fixture.Create<UserDto>();

            var res = await usersController.Register(user);
            var badRequest = Assert.IsType<BadRequestObjectResult>(res);
        }

        [Fact]
        public async void Authenticate_HappyPath() {
            userRepo.Setup(x => x.Authenticate(It.IsAny<String>(), It.IsAny<String>()))
                .ReturnsAsync((String userName, String password) => 
                    fixture.Build<User>()
                    .With(x => x.Username, userName)
                    .Create());
            var user = fixture.Create<UserDto>();
            
            var res = await usersController.Authenticate(user);
            var okResult = Assert.IsType<OkObjectResult>(res);
        }

        [Fact]
        public async void Authenticate_UserIsNull_ReturnsBadRequest() {
            userRepo.Setup(x => x.Authenticate(It.IsAny<String>(), It.IsAny<String>()))
                .ReturnsAsync((String userName, String password) => null);
                    
            var user = fixture.Create<UserDto>();

            var res = await usersController.Authenticate(user);
            Assert.IsType<BadRequestObjectResult>(res);
        }

        [Fact]
        public async void Delete_HappyPath() {
            userRepo.Setup(x => x.Remove(It.IsAny<int>()))
                .ReturnsAsync(() => null);

            var id = fixture.Create<int>();
            var res = await usersController.Delete(id);
            Assert.IsType<OkResult>(res);
        }

        [Fact]
        public async void Update_HappyPath() {
            userRepo.Setup(x => x.Update(It.IsAny<User>(), It.IsAny<String>()))
                .ReturnsAsync(() => null);

            var userDto = fixture.Create<UserDto>();
            var res = await usersController.Update(userDto.Id, userDto);
            Assert.IsType<OkResult>(res);
        }

        [Fact]
        public async void Update_ThrowsException_ReturnsBadRequestWithMessage()
        {
            var message = fixture.Create<String>();
            userRepo.Setup(x => x.Update(It.IsAny<User>(), It.IsAny<String>()))
                .ThrowsAsync(new AppException(message));

            var userDto = fixture.Create<UserDto>();
            var res = await usersController.Update(userDto.Id, userDto);
            var badRequest = Assert.IsType<BadRequestObjectResult>(res);
        }
    }
}