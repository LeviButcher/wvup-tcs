using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using tcs_service.Services;
using tcs_service.Services.Interfaces;
using Xunit;
using WireMock.Server;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using tcs_service_test.Helpers;
using tcs_service.Helpers;
using tcs_service.Models.DTO;

namespace tcs_service_test.Services
{
    public class LiveBannerServiceTest : IDisposable
    {
        private readonly WireMockServer server;
        private readonly IBannerService bannerApi;

        public LiveBannerServiceTest()
        {
            server = FluentMockServer.Start(new FluentMockServerSettings()
            {
                Urls = new[] { "http://localhost:9000/" }
            });
            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri("http://localhost:9000/")
            };

            Mock<IHttpClientFactory> mockHttpFactory = new Mock<IHttpClientFactory>();
            mockHttpFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(() => httpClient);
            bannerApi = new LiveBannerService(mockHttpFactory.Object);
        }

        public void Dispose()
        {
            server.Dispose();
        }


        [Theory]
        [InlineData("lbutche3@wvup.edu")]
        [InlineData("9109213123")]
        public async Task GetBannerInfo_StudentIdentifier_ShouldCallCorrectApiAndReturnBackStudentInfo(string identifier)
        {
            var bannerInfo = new BannerPersonInfo()
            {
                EmailAddress = identifier,
                WVUPID = Utils.ParseOrDefault(identifier, 11243213),
                FirstName = "Billy",
                LastName = "Loel",
                Teacher = false
            };

            var cleanedIdentifier = identifier.Split("@")[0];

            server
            .Given(Request.Create().WithPath($"/student/{cleanedIdentifier}").UsingGet())
            .RespondWith(
                Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(bannerInfo)
            );

            var studentInfo = await bannerApi.GetBannerInfo(identifier);

            Assert.NotNull(studentInfo);
            Assert.False(studentInfo.Teacher);
        }

        [Theory]
        [InlineData("teacher@wvup.edu")]
        [InlineData("112452354")]
        public async Task GetBannerInfo_TeacherEmail_ShouldCallCorrectApiAndReturnBackTeacherInfo(string identifier)
        {
            var bannerInfo = new BannerPersonInfo()
            {
                EmailAddress = identifier,
                WVUPID = Utils.ParseOrDefault(identifier, 11243213),
                FirstName = "Barry",
                LastName = "LoppySomph",
                Teacher = true
            };
            var cleanedIdentifier = identifier.Split("@")[0];

            server
            .Given(Request.Create().WithPath($"/teacher/{cleanedIdentifier}").UsingGet())
            .RespondWith(
                Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(bannerInfo)
            );

            var teacherInfo = await bannerApi.GetBannerInfo(identifier);

            Assert.NotNull(teacherInfo);
            Assert.True(teacherInfo.Teacher);
        }

        [Fact]
        public async Task GetBannerInfo_PersonNotFound_ShouldThrowTCSExceptionWithMessage()
        {
            var email = "whoami@wvup.edu";
            var cleanedEmail = email.Split("@")[0];

            server
            .Given(Request.Create().WithPath($"/teacher/{cleanedEmail}").UsingGet())
            .RespondWith(
                Response.Create()
                .WithStatusCode(404)
                .WithHeader("Content-Type", "application/json")
            );

            var exception = await Assert.ThrowsAsync<TCSException>(async () => await bannerApi.GetBannerInfo(email));
            Assert.Equal($"Could not find information on: {email}", exception.Message);
        }

        [Fact]
        public async Task GetBannerInfo_BannerIsDown_ShouldThrowTCSExceptionWithMessage()
        {
            var email = "whoami@wvup.edu";
            var cleanedEmail = email.Split("@")[0];

            server
            .Given(Request.Create().WithPath($"/teacher/{cleanedEmail}").UsingGet())
            .RespondWith(
                Response.Create()
                .WithStatusCode(503)
                .WithHeader("Content-Type", "application/json")
            );

            var exception = await Assert.ThrowsAsync<TCSException>(async () => await bannerApi.GetBannerInfo(email));
            Assert.Equal($"Banner is currently down", exception.Message);
        }

        [Fact]
        public async Task GetStudentGrade_ShouldCallCorrectApiAndReturnStudentGrade()
        {
            var studentId = 6789613;
            var semesterCode = 201901;
            var classCRN = 1476;

            server.Given(Request.Create()
                .WithPath($"/student/{studentId}/{semesterCode}/{classCRN}")
                .UsingGet())
            .RespondWith(
                Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(new { FinalGrade = "A", MidtermGrade = "B", CRN = classCRN, SubjectCode = "STEM", CourseNumber = "101" })
            );
            var result = await bannerApi.GetStudentGrade(studentId, classCRN, semesterCode);
            Assert.Equal(classCRN, result.CRN);
        }

        [Fact]
        public async Task GetStudentGrade_BannerApiReturnsBack404_ShouldThrowTCSExceptionWithMessage()
        {
            var studentId = 6789613;
            var semesterCode = 201901;
            var classCRN = 1476;

            server.Given(Request.Create()
                .WithPath($"/student/{studentId}/{semesterCode}/{classCRN}")
                .UsingGet())
            .RespondWith(
                Response.Create()
                .WithStatusCode(404)
            );
            var result = await bannerApi.GetStudentGrade(studentId, classCRN, semesterCode);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetStudentGrade_BannerApiReturnsBadStatusCode_ShouldReturnNull()
        {
            var studentId = 6789613;
            var semesterCode = 201901;
            var classCRN = 1476;

            server.Given(Request.Create()
                .WithPath($"/student/{studentId}/{semesterCode}/{classCRN}")
                .UsingGet())
            .RespondWith(
                Response.Create()
                .WithStatusCode(500)
            );

            var result = await bannerApi.GetStudentGrade(studentId, classCRN, semesterCode);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetStudentGrade_BannerApiReturnsNoGrade_ShouldReturnNone()
        {
            var studentId = 6789613;
            var semesterCode = 201901;
            var classCRN = 1476;

            server.Given(Request.Create()
            .WithPath($"/student/{studentId}/{semesterCode}/{classCRN}")
            .UsingGet())
            .RespondWith(
                Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(new { FinalGrade = "", MidtermGrade = "", CRN = classCRN, SubjectCode = "STEM", CourseNumber = "101" })
            );

            var result = await bannerApi.GetStudentGrade(studentId, classCRN, semesterCode);
            Assert.Null(result);
        }
    }
}