using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using tcs_service;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;
using tcs_service.Models;
using Newtonsoft.Json;

namespace tcs_integration.Controllers
{
    [Collection("Integration")]
    public class ReasonsControllerTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public ReasonsControllerTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        private async Task<UserDto> Login(HttpClient client)
        {
            var logInResponse = await client.PostAsJsonAsync("api/users/authenticate", new { username = "tcs", password = "Develop@90" });
            Assert.Equal(HttpStatusCode.OK, logInResponse.StatusCode);
            var user = await logInResponse.Content.ReadAsAsync<UserDto>();
            return user;
        }

        [Fact]
        public async void GET_reasons_ShouldReturnListOfReasons()
        {
            var client = _factory.CreateClient();
            var user = await Login(client);
            var request = new HttpRequestMessage(HttpMethod.Get, "api/reasons");
            request.Headers.Add("Authorization", $"Bearer {user.Token}");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var reasons = await response.Content.ReadAsAsync<IEnumerable<Reason>>();

            Assert.NotNull(reasons);
        }

        [Fact]
        public async void GET_reasons_id_ShouldReturnReasonWithMatchingId()
        {
            var reasonId = 1;
            var client = _factory.CreateClient();
            var user = await Login(client);
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/reasons/{reasonId}");
            request.Headers.Add("Authorization", $"Bearer {user.Token}");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var reason = await response.Content.ReadAsAsync<Reason>();

            Assert.Equal(reasonId, reason.Id);
        }

        [Fact]
        public async void GET_reasons_active_ShouldReturnListOfReasonsThatAreNotDeleted()
        {
            var client = _factory.CreateClient();
            var user = await Login(client);
            var request = new HttpRequestMessage(HttpMethod.Get, "api/reasons/active");
            request.Headers.Add("Authorization", $"Bearer {user.Token}");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var reasons = await response.Content.ReadAsAsync<IEnumerable<Reason>>();

            Assert.All(reasons, r => Assert.Equal(false, r.Deleted));
        }

        [Fact]
        public async void POST_reasons_ShouldReturn201WithCreatedReason()
        {
            var client = _factory.CreateClient();
            var user = await Login(client);
            var request = new HttpRequestMessage(HttpMethod.Post, "api/reasons");
            request.Headers.Add("Authorization", $"Bearer {user.Token}");
            request.Content = new StringContent(JsonConvert.SerializeObject(new Reason { Name = "Bone Use" }));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var reason = await response.Content.ReadAsAsync<Reason>();
            Assert.Equal("Bone Use", reason.Name);
            Assert.Equal(false, reason.Deleted);
        }

        [Fact]
        public async void PUT_reasons_id_ShouldReturn200WithUpdatedReason()
        {
            var client = _factory.CreateClient();
            var user = await Login(client);
            var request = new HttpRequestMessage(HttpMethod.Put, "api/reasons/1");
            request.Headers.Add("Authorization", $"Bearer {user.Token}");
            request.Content = new StringContent(JsonConvert.SerializeObject(new Reason { Id = 1, Name = "Computation Use", Deleted = true }));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var reason = await response.Content.ReadAsAsync<Reason>();
            Assert.Equal("Computation Use", reason.Name);
            Assert.Equal(true, reason.Deleted);
        }
    }
}