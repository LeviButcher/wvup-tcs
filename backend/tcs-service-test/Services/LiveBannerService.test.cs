using System;
using System.Net;
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

namespace tcs_service_test.Services
{
    public class LiveBannerServiceTest : IDisposable
    {
        private readonly FluentMockServer server;
        private readonly IBannerService bannerApi;

        public LiveBannerServiceTest()
        {
            server = FluentMockServer.Start(new FluentMockServerSettings()
            {
                Urls = new[] { "http://localhost:9000/" }
            });
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://localhost:9000/");
            Mock<IHttpClientFactory> mockHttpFactory = new Mock<IHttpClientFactory>();
            mockHttpFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(() => httpClient);
            Mock<IMapper> mockMapper = new Mock<IMapper>();
            bannerApi = new LiveBannerService(mockHttpFactory.Object, mockMapper.Object);
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
                Email = identifier,
                Id = Utils.ParseOrDefault(identifier, 11243213),
                FirstName = "Billy",
                LastName = "Loel",
                Teacher = false
            };

            server
            .Given(Request.Create().WithPath($"/student/{identifier}").UsingGet())
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
                Email = identifier,
                Id = Utils.ParseOrDefault(identifier, 11243213),
                FirstName = "Barry",
                LastName = "LoppySomph",
                Teacher = true
            };

            server
            .Given(Request.Create().WithPath($"/teacher/{identifier}").UsingGet())
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
    }
}