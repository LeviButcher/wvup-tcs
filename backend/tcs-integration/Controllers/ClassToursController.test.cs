using tcs_service;
using System.Net;
using System.Net.Http;
using Xunit;
using System.Collections.Generic;
using tcs_service.Models;
using tcs_integration.test_utils;

namespace tcs_integration.Controllers
{
    [Collection("Integration")]
    public class ClassToursControllerTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public ClassToursControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async void GET_classtours_id_ShouldReturnClassTourWithMatchingId()
        {
            var classTourId = 1;
            var client = _factory.CreateClient();
            var user = await Utils.Login(client);
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/classtours/{classTourId}");
            request.Headers.Add("Authorization", $"Bearer {user.Token}");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var tours = await response.Content.ReadAsAsync<ClassTour>();

            Assert.Equal(classTourId, tours.Id);
        }

        [Fact]
        public async void GET_classtours_id_ShouldReturnClassTourWithMatchingId()
        {
            var classTourId = 1;
            var client = _factory.CreateClient();
            var user = await Utils.Login(client);
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/classtours/{classTourId}");
            request.Headers.Add("Authorization", $"Bearer {user.Token}");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var tours = await response.Content.ReadAsAsync<ClassTour>();

            Assert.Equal(classTourId, tours.Id);
        }
    }

  
}