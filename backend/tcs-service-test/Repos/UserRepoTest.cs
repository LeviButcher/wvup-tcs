using AutoFixture;
using AutoFixture.AutoMoq;
using System;
using tcs_service.Exceptions;
using tcs_service.Models;
using tcs_service.Repos;
using Xunit;
using tcs_service_test.Helpers;
using Moq;
using Microsoft.Extensions.Options;
using tcs_service.Helpers;
using tcs_service.EF;

namespace tcs_service_test.Repos
{
    public class UserRepoTest : IDisposable
    {
        UserRepo userRepo;
        string dbName = "UserRepoTest";

        readonly string secret = "supersecretpassphrasethatissuperlongbecauseithastobe";

        IFixture fixture;

        Mock<IOptions<AppSettings>> mockAppSettings;

        TCSContext db;

        public UserRepoTest()
        {
            AppSettings appSettings = new AppSettings() { Secret = secret };
            mockAppSettings = new Mock<IOptions<AppSettings>>();
            mockAppSettings.Setup(ap => ap.Value).Returns(appSettings);
            var dbOptions = DbInMemory.getDbInMemoryOptions(dbName);
            db = new TCSContext(dbOptions);
            userRepo = new UserRepo(dbOptions, mockAppSettings.Object);
            fixture = new Fixture()
              .Customize(new AutoMoqCustomization());

            // Sets all users created by autofixture to not have properties set
            fixture.Customize<User>((ob) => ob.Without(x => x.Id).Without(x => x.PasswordHash).Without(x => x.PasswordSalt));
        }

        public void Dispose()
        {
            db.Database.EnsureDeleted();
        }

        [Fact]
        public async void CreateUser_HappyPath()
        {
            var user = fixture.Create<User>();
            var password = fixture.Create<string>();

            var res = await userRepo.Create(user, password);

            Assert.Equal(res.FirstName, user.FirstName);
        }

        [Fact]
        public async void CreateUser_MissingPassword_ThrowsException()
        {
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
        public async void Authenticate_HappyPath()
        {
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
        public async void Authenticate_MissingParams_ReturnNull(string userName, string password)
        {
            var res = await userRepo.Authenticate(userName, password);
            Assert.Null(res);
        }

        [Theory]
        [InlineData("Bobby", "Develop@90")]
        public async void Authenticate_UserDoesNotExist_ReturnNull(string userName, string password)
        {
            var res = await userRepo.Authenticate(userName, password);
            Assert.Null(res);
        }

        [Theory]
        [InlineData("Develop@90", "aal;ksjdf")]
        [InlineData("Develop@90", "Develop@91")]
        public async void Authenticate_PasswordDoesNotMatchHash_ReturnNull(string password, string incorrectPassword)
        {
            var user = fixture.Create<User>();
            await userRepo.Create(user, password);

            var res = await userRepo.Authenticate(user.Username, incorrectPassword);

            Assert.Null(res);
        }

        //change username
        [Fact]
        public async void UpdateUser_HappyPath()
        {
            const string updateUserName = "updatedUser";
            var user = fixture.Create<User>();
            var password = fixture.Create<String>();
            var createdUser = await userRepo.Create(user, password);
            var originalName = createdUser.Username;
            var updatedUser = createdUser;

            updatedUser.Username = updateUserName;
            var updateRes = await userRepo.Update(updatedUser);

            Assert.NotEqual(originalName, updateRes.Username);
            Assert.Equal(updateRes.Username, updateUserName);
        }

        [Fact]
        public async void UpdateUser_EmptyUserName_ShouldThrowException()
        {
            const string updateUserName = null;
            var user = fixture.Create<User>();
            var password = fixture.Create<String>();
            var createdUser = await userRepo.Create(user, password);

            var originalName = createdUser.Username;
            var updatedUser = createdUser;

            updatedUser.Username = updateUserName;
            await Assert.ThrowsAsync<AppException>(() => userRepo.Update(updatedUser));
        }

        [Fact]
        public async void UpdateUser_ChangePassword_HappyPath()
        {
            var user = fixture.Create<User>();
            var password = fixture.Create<String>();
            var createdUser = await userRepo.Create(user, password);
            var oldPassword = createdUser.PasswordHash;

            var newPassword = fixture.Create<String>();
            var updatedUser = await userRepo.Update(createdUser, newPassword);

            Assert.NotEqual(oldPassword, updatedUser.PasswordHash);
        }

        [Fact]
        public async void UpdateUser_ChangePasswordWithNull_DoesNotChangeHash()
        {
            var user = fixture.Create<User>();
            var password = fixture.Create<String>();
            var createdUser = await userRepo.Create(user, password);
            var oldPassword = createdUser.PasswordHash;

            var newPassword = "";
            var updatedUser = await userRepo.Update(createdUser, newPassword);

            Assert.Equal(oldPassword, updatedUser.PasswordHash);
        }

        // Gave real names to make more readable
        // Have to create a copy to give a new reference so to not automatically updating
        // the tom stored in memory db's Username
        [Fact]
        public async void UpdateUser_UsernameAlreadyTaken_ThrowsException()
        {
            var password = fixture.Create<String>();

            var bob = fixture.Build<User>().With(x => x.Username, "bob").Create();
            bob = await userRepo.Create(bob, password);
            var tom = fixture.Build<User>().With(x => x.Username, "tom").Create();
            tom = await userRepo.Create(tom, password);

            var newTom = tom.Copy();
            newTom.Username = "bob";

            await Assert.ThrowsAsync<AppException>(() => userRepo.Update(newTom));
        }

        [Fact]
        public async void RemoveUser_HappyPath()
        {
            var password = fixture.Create<String>();

            var bob = fixture.Build<User>().Create();
            bob = await userRepo.Create(bob, password);

            await userRepo.Remove(x => x.Id == bob.Id);
            var found = await userRepo.Find(x => x.Id == bob.Id);

            Assert.Null(found);
        }
    }
}