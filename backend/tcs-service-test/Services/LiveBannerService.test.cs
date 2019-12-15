using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Newtonsoft.Json;
using tcs_service.Services;
using tcs_service.Services.Interfaces;
using Xunit;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc.WebApiCompatShim;

namespace tcs_service_test.Services
{
    public class LiveBannerServiceTest
    {
        // Taken From: https://stackoverflow.com/questions/54227487/how-to-mock-the-new-httpclientfactory-in-net-core-2-1-using-moq
        public class DelegatingHandlerStub : DelegatingHandler
        {
            private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handlerFunc;
            public DelegatingHandlerStub()
            {
                _handlerFunc = (request, cancellationToken) => Task.FromResult(request.CreateResponse(HttpStatusCode.OK));
            }

            public DelegatingHandlerStub(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handlerFunc)
            {
                _handlerFunc = handlerFunc;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return _handlerFunc(request, cancellationToken);
            }
        }

        public LiveBannerServiceTest()
        {
        }



        [Fact]
        public async Task GetBannerInfo_StudentEmail_ShouldCallHttpClientWithCorrectArgsAndReturnBackStudentInfo()
        {
            // await bannerApi.GetAsync($"student/{studentId}/{termCode}/{crn}");
            var email = "lbutche3@wvup.edu";
            var bannerInfo = new BannerPersonInfo()
            {
                Email = email,
                FirstName = "Billy",
                LastName = "Loel",
                Teacher = false
            };

            var handlerStub = new DelegatingHandlerStub((request, cancellationToken) =>
            {
                var response = request.CreateResponse(HttpStatusCode.OK, bannerInfo);
                return Task.FromResult(response);
            });
            var httpClient = new HttpClient(handlerStub);
            httpClient.BaseAddress = new Uri("https://banner.wvup.edu/");
            Mock<IHttpClientFactory> mockHttpFactory = new Mock<IHttpClientFactory>();
            mockHttpFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(() => httpClient);
            Mock<IMapper> mockMapper = new Mock<IMapper>();
            var bannerApi = new LiveBannerService(mockHttpFactory.Object, mockMapper.Object);

            var studentInfo = await bannerApi.GetBannerInfo(email);

            Assert.NotNull(studentInfo);
            Assert.False(studentInfo.Teacher);
        }
    }
}