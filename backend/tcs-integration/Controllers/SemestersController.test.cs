using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using tcs_integration.test_utils;
using tcs_service;
using tcs_service.Models;
using Xunit;

namespace tcs_integration.Controllers
{
    [Collection("Integration")]
    public class SemestersControllerTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        public SemestersControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async void GET_semesters_ShouldReturnListOfSemesetrs()
        {
            var client = _factory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "api/semesters");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var semesters = await response.Content.ReadAsAsync<IEnumerable<Semester>>();

            Assert.NotNull(semesters);
        }
    }
}
