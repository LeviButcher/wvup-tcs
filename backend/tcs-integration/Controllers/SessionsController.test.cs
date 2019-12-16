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

            Assert.Equal(sessionId, session.ID);
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
                SelectedClasses = new List<Class>() { new Class { CRN = 4, DepartmentCode = 2, Name = "ENGLISH LIT", ShortName = "English 101" } } ,
                SelectedReasons = new List<Reason>() { new Reason { Id = 2, Name = "Computer Use", Deleted = true} }  
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
        public async void PUT_sessions_id_ShouldReturn200WithUpdatedSession()
        {
            var client = _factory.CreateClient();
            var user = await Login(client);
            var request = new HttpRequestMessage(HttpMethod.Put, "api/sessions/1");
            request.Headers.Add("Authorization", $"Bearer {user.Token}");
            request.Content = new StringContent(JsonConvert.SerializeObject(new Session {ID = 1, PersonId = 5, Tutoring = false, InTime = DateTime.Now, OutTime = null }));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var session = await response.Content.ReadAsAsync<Session>();
            Assert.False( session.Tutoring);
            Assert.Equal(5, session.PersonId);
        }

        [Fact]
        public async void PUT_signout_id_ShouldReturn200WithSignedOutSession()
        {
            var client = _factory.CreateClient();
            var user = await Login(client);
            var request = new HttpRequestMessage(HttpMethod.Put, "api/sessions/signout/175");
            request.Headers.Add("Authorization", $"Bearer {user.Token}");
            request.Content = new StringContent(JsonConvert.SerializeObject(new Session {  OutTime = DateTime.Now }));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var session = await response.Content.ReadAsAsync<Session>();
            Assert.NotNull(session.OutTime);
        }

        [Fact]
        public async void DELETE_sessions_id_ShouldReturn200WithUpdatedSession()
        {
            var client = _factory.CreateClient();
            var user = await Login(client);
            var request = new HttpRequestMessage(HttpMethod.Delete, "api/sessions/27");
            request.Headers.Add("Authorization", $"Bearer {user.Token}");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var session = await response.Content.ReadAsAsync<Session>();
            Assert.Equal(27, session.ID);
            
            request = new HttpRequestMessage(HttpMethod.Get, $"api/sessions/27");
            request.Headers.Add("Authorization", $"Bearer {user.Token}");
            response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
           
        }

    }
}
