using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using tcs_service;
using tcs_service.Helpers;
using tcs_service.Models;
using Xunit;

namespace tcs_integration.Controllers
{
    [Collection("Integration")]
    public class SessionsControllerTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private IFixture fixture;

        public SessionsControllerTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            fixture = new Fixture().Customize(new AutoMoqCustomization());
        }

        private async Task<UserDto> Login(HttpClient client)
        {
            var logInResponse = await client.PostAsJsonAsync("api/users/authenticate", new { username = "tcs", password = "Develop@90" });
            Assert.Equal(HttpStatusCode.OK, logInResponse.StatusCode);
            var user = await logInResponse.Content.ReadAsAsync<UserDto>();
            return user;
        }

        [Fact]
        public async void GET_sessions_ShouldReturnListOfSessions()
        {
            var client = _factory.CreateClient();
            var user = await Login(client);
            var request = new HttpRequestMessage(HttpMethod.Get, "api/sessions");
            request.Headers.Add("Authorization", $"Bearer {user.Token}");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var sessions = await response.Content.ReadAsAsync<Paging<Session>>();

            Assert.NotNull(sessions);
        }

        [Fact]
        public async void POST_sessions_ShouldReturn201WithCreatedSession()
        {
            var client = _factory.CreateClient();
            var session = new Session()
            {
                InTime = DateTime.Now,
                PersonId = 1,
                Tutoring = true
            };
            var user = await Login(client);
            var request = new HttpRequestMessage(HttpMethod.Post, "api/sessions");
            request.Headers.Add("Authorization", $"Bearer {user.Token}");
            request.Content = new StringContent(JsonConvert.SerializeObject(session));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var createdSession = await response.Content.ReadAsAsync<Session>();
            Assert.Equal(1, createdSession.PersonId );
            Assert.True( session.Tutoring);
        }

    }
}
