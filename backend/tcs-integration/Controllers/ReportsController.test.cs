using System.Net.Http.Headers;
using tcs_service;
using System.Net;
using System.Net.Http;
using Xunit;
using System.Collections.Generic;
using tcs_service.Models;
using Newtonsoft.Json;
using tcs_integration.test_utils;
using tcs_service.Models.DTO;

namespace tcs_integration.Controllers
{
    [Collection("Integration")]
    public class ReportsControllerTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public ReportsControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async void GET_SuccessReport_ShouldWork()
        {
            var client = _factory.CreateClient();
            var user = await Utils.Login(client);
            var request = new HttpRequestMessage(HttpMethod.Get, "api/reports/success/201901/");
            request.Headers.Add("Authorization", $"Bearer {user.Token}");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var reasons = await response.Content.ReadAsAsync<IEnumerable<ClassSuccessCountDTO>>();
            Assert.NotEmpty(reasons);
        }
    }
}