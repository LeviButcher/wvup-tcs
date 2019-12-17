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
using tcs_service.Models.DTOs;
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
        public async void GET_sessions_id_ShouldReturnSessionWithMatchingId()
        {
            var sessionId = 1;
            var client = _factory.CreateClient();
            var user = await Login(client);
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/sessions/{sessionId}");
            request.Headers.Add("Authorization", $"Bearer {user.Token}");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var session = await response.Content.ReadAsAsync<Session>();

            Assert.Equal(sessionId, session.Id);
        }
        
        [Fact]
        public async void GET_outtime_null_ShouldReturnListOfSessionsWhereOutTimeIsNull()
        {
            var client = _factory.CreateClient();
            var user = await Login(client);
            var request = new HttpRequestMessage(HttpMethod.Get, "api/sessions/signedin");
            request.Headers.Add("Authorization", $"Bearer {user.Token}");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var sessions = await response.Content.ReadAsAsync<IEnumerable<Session>>();

            Assert.All(sessions, s => Assert.Null( s.OutTime ));
        }

        [Fact]
        public async void POST_sessions_ShouldReturn201WithCreatedSession()
        {
            var client = _factory.CreateClient();
            var session = new SessionCreateDTO()
            {
                InTime = DateTime.Now,
                PersonId = 1,
                Tutoring = true,
                SelectedClasses = new List<int>() { 3, 4 } ,
                SelectedReasons = new List<int>() { 2 }  
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

        [Fact]
        public async void POST_session_withnoselectedclasses_ShouldReturn400()
        {
            var client = _factory.CreateClient();
            var session = new SessionCreateDTO()
            {
                InTime = DateTime.Now,
                PersonId = 1,
                Tutoring = true,
                SelectedClasses = new List<int>() {  },
                SelectedReasons = new List<int>() { 2 }
            };
            var user = await Login(client);
            var request = new HttpRequestMessage(HttpMethod.Post, "api/sessions");
            request.Headers.Add("Authorization", $"Bearer {user.Token}");
            request.Content = new StringContent(JsonConvert.SerializeObject(session));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async void POST_session_withnoselectedreasons_tutoringtrue_ShouldReturn201WithCreatedSession()
        {
            var client = _factory.CreateClient();
            var session = new SessionCreateDTO()
            {
                InTime = DateTime.Now,
                PersonId = 1,
                Tutoring = true,
                SelectedClasses = new List<int>() { 2 },
                SelectedReasons = new List<int>() { }
            };
            var user = await Login(client);
            var request = new HttpRequestMessage(HttpMethod.Post, "api/sessions");
            request.Headers.Add("Authorization", $"Bearer {user.Token}");
            request.Content = new StringContent(JsonConvert.SerializeObject(session));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var createdSession = await response.Content.ReadAsAsync<Session>();
            Assert.Equal(1, createdSession.PersonId);
            Assert.True(session.Tutoring);
        }

        [Fact]
        public async void POST_session_withnoselectedreasons_tutoringfalse_ShouldReturn400()
        {
            var client = _factory.CreateClient();
            var session = new SessionCreateDTO()
            {
                InTime = DateTime.Now,
                PersonId = 1,
                Tutoring = false,
                SelectedClasses = new List<int>() { 3 },
                SelectedReasons = new List<int>() {  }
            };
            var user = await Login(client);
            var request = new HttpRequestMessage(HttpMethod.Post, "api/sessions");
            request.Headers.Add("Authorization", $"Bearer {user.Token}");
            request.Content = new StringContent(JsonConvert.SerializeObject(session));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }


        [Fact]
        public async void PUT_sessions_id_ShouldReturn200WithUpdatedSession()
        {
            var client = _factory.CreateClient();
            var user = await Login(client);
            var request = new HttpRequestMessage(HttpMethod.Put, "api/sessions/3");
            var sessionDTO = new SessionCreateDTO()
            {
                Id = 3,
                InTime = DateTime.Now,
                PersonId = 8,
                Tutoring = true,
                SelectedClasses = new List<int>() { 3 },
                SelectedReasons = new List<int>() {  }
            };
            request.Headers.Add("Authorization", $"Bearer {user.Token}");
            request.Content = new StringContent(JsonConvert.SerializeObject(sessionDTO));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var session = await response.Content.ReadAsAsync<Session>();
            Assert.Equal(8, session.PersonId);
        }

        [Fact]
        public async void PUT_signout_id_ShouldReturn200WithSignedOutSession()
        {
            var client = _factory.CreateClient();
            var user = await Login(client);
            var request = new HttpRequestMessage(HttpMethod.Put, "api/sessions/signout/61");
            request.Headers.Add("Authorization", $"Bearer {user.Token}");
            request.Content = new StringContent(JsonConvert.SerializeObject(new Session {  OutTime = DateTime.Now }));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var session = await response.Content.ReadAsAsync<Session>();
            Assert.NotNull(session.OutTime);
        }

        [Fact]
        public async void PUT_signout_studentnotsignedin_ShouldReturn400()
        {
            var client = _factory.CreateClient();
            var user = await Login(client);
            var request = new HttpRequestMessage(HttpMethod.Put, "api/sessions/signout/35");
            request.Headers.Add("Authorization", $"Bearer {user.Token}");
            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async void DELETE_sessions_id_ShouldReturn200WithDeletedSession()
        {
            var client = _factory.CreateClient();
            var user = await Login(client);
            var request = new HttpRequestMessage(HttpMethod.Delete, "api/sessions/27");
            request.Headers.Add("Authorization", $"Bearer {user.Token}");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var session = await response.Content.ReadAsAsync<Session>();
            Assert.Equal(27, session.Id);
            
            request = new HttpRequestMessage(HttpMethod.Get, $"api/sessions/27");
            request.Headers.Add("Authorization", $"Bearer {user.Token}");
            response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
           
        }

    }
}
