using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using tcs_service.Controllers;
using tcs_service.EF;
using tcs_service.Exceptions;
using tcs_service.Models;
using tcs_service.Repos;
using tcs_service.Repos.Interfaces;
using Xunit;

namespace tcs_service_test
{
  public class DbInMemory {
    public static DbContextOptions getDbInMemoryOptions(string dbName)
    {
        var options = new DbContextOptionsBuilder<TCSContext>()
              .UseInMemoryDatabase(databaseName: dbName)
              .Options;
        return options;
    }
  }

  public class UserRepoTest : IDisposable
  {
    UserRepo userRepo;
    string dbName = "UserRepoTest";
    IFixture fixture;

    public UserRepoTest() {
      userRepo = new UserRepo(DbInMemory.getDbInMemoryOptions(dbName));
      fixture = new Fixture()
        .Customize(new AutoMoqCustomization());
    }

    public void Dispose()
    {
        userRepo = null;
        fixture = null;
    }

    // Expect user to be successfuly created and be in repo
    [Fact]
    public async void CreateUser_HappyPath() {
      var user = fixture.Create<User>();
      var password = fixture.Create<string>();

      var res = await userRepo.Create(user, password);

      Assert.Equal(res.FirstName, user.FirstName);
    }

    [Fact]
    public async void CreateUser_MissingPassword_ThrowsException() {
      var user = fixture.Create<User>();
      string password = null;

      await Assert.ThrowsAsync<AppException>(() => userRepo.Create(user, password));
    }

    [Fact]
    public async void CreateUser_UserNameTaken_ThrowsException()
    {
      var user = fixture.Freeze<User>();
      var repeatUser = fixture.Freeze<User>();
      var password = fixture.Create<string>();
      await userRepo.Create(user, password);
      await Assert.ThrowsAsync<AppException>(() => userRepo.Create(repeatUser, password));
    }

    [Fact]
    public async void Authenticate_HappyPath() {
      var user = fixture.Create<User>();
      var password = fixture.Create<string>();
      await userRepo.Create(user, password);
      var res = await userRepo.Authenticate(user.Username, password);
      Assert.Equal(res.Username, user.Username);
    }

    [Theory]
    [InlineData("Bobby", null)]
    [InlineData(null, "Develop@90")]
    [InlineData(null, null)]
    public async void Authenticate_MissingParams_ReturnNull(string userName, string password) {
      var res = await userRepo.Authenticate(userName, password);
      Assert.Null(res);
    }

    [Theory]
    [InlineData("Bobby","Develop@90")]
    public async void Authenticate_UserDoesNotExist_ReturnNull(string userName, string password)
    {
      var res = await userRepo.Authenticate(userName, password);
      Assert.Null(res);
    }

    [Theory]
    [InlineData("Develop@90", "aal;ksjdf")]
    [InlineData("Develop@90", "Develop@91")]
    public async void Authenticate_PasswordDoesNotMatchHash_ReturnNull(string password, string incorrectPassword) {
      var user = fixture.Create<User>();
      await userRepo.Create(user, password);

      var res = await userRepo.Authenticate(user.Username, incorrectPassword);

      Assert.Null(res);
    }
  }
}