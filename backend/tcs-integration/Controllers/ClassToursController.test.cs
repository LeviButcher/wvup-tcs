using tcs_service;
using System.Net;
using System.Net.Http;
using Xunit;
using System.Collections.Generic;
using tcs_service.Models;
using tcs_integration.test_utils;
using System;
using tcs_service.Helpers;
using System.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;

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
        public async void GET_classtours_invalidId_ShouldReturn404()
        {
            var classTourId = 600;
            var client = _factory.CreateClient();
            var user = await Utils.Login(client);
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/classtours/{classTourId}");
            request.Headers.Add("Authorization", $"Bearer {user.Token}");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData("08/01/19", "08/30/19")]
        [InlineData("01/01/18", "10/30/19")]
        public async void GET_classtoursBetweenDates_ShouldReturn200WithOnlyToursBetweenDates(string start, string end)
        {
            var client = _factory.CreateClient();
            var user = await Utils.Login(client);
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/classtours?start={start}&end={end}");
            request.Headers.Add("Authorization", $"Bearer {user.Token}");

            var response = await client.SendAsync(request);

            var tours = await response.Content.ReadAsAsync<Paging<ClassTour>>();             
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            foreach(var tour in tours.Data)
            {
                Assert.True(tour.DayVisited >= Convert.ToDateTime(start) && tour.DayVisited <= Convert.ToDateTime(end));
            }           
        }

        [Fact]
        public async void PUT_classtours_idsMatchAndValidData_ShouldReturn200()
        {
            var tourId = 1;
            var client = _factory.CreateClient();
            var tour = new ClassTour()
            {
                DayVisited = DateTime.Now,
                Name = "Wahama High School",
                NumberOfStudents = 23,
                Id = tourId
            };
            var user = await Utils.Login(client);
            var request = new HttpRequestMessage(HttpMethod.Put, $"api/classtours/{tourId}");
            request.Headers.Add("Authorization", $"Bearer {user.Token}");
            request.Content = new StringContent(JsonConvert.SerializeObject(tour));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var updatedTour = await response.Content.ReadAsAsync<ClassTour>();
            Assert.Equal("Wahama High School", updatedTour.Name);
        }

        [Fact]
        public async void PUT_classtours_idsMatchAndInvalidData_ShouldReturn400()
        {
            var tourId = 1;
            var client = _factory.CreateClient();
            var tour = new ClassTour()
            {
                DayVisited = DateTime.Now,
                Name = "",
                NumberOfStudents = 23,
                Id = tourId
            };
            var user = await Utils.Login(client);
            var request = new HttpRequestMessage(HttpMethod.Put, $"api/classtours/{tourId}");
            request.Headers.Add("Authorization", $"Bearer {user.Token}");
            request.Content = new StringContent(JsonConvert.SerializeObject(tour));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async void PUT_classtours_idsDoNotMatch_ShouldReturn500WithMessage()
        {
            var tourId = 1;
            var wrongTourId = 5;
            var client = _factory.CreateClient();
            var tour = new ClassTour()
            {
                DayVisited = DateTime.Now,
                Name = "Tour Name",
                NumberOfStudents = 23,
                Id = wrongTourId
            };
            var user = await Utils.Login(client);
            var request = new HttpRequestMessage(HttpMethod.Put, $"api/classtours/{tourId}");
            request.Headers.Add("Authorization", $"Bearer {user.Token}");
            request.Content = new StringContent(JsonConvert.SerializeObject(tour));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.SendAsync(request);

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            var errorMessage = await response.Content.ReadAsAsync<ErrorMessage>();
            Assert.Equal("Class Tour does not exist", errorMessage.Message);
        }

        [Fact]
        public async void POST_classtour_validData_ShouldReturn201WithClassTour()
        {
            var client = _factory.CreateClient();
            var user = await Utils.Login(client);
            var request = new HttpRequestMessage(HttpMethod.Post, "api/classtours");
            request.Headers.Add("Authorization", $"Bearer {user.Token}");
            request.Content = new StringContent(JsonConvert.SerializeObject(new ClassTour { DayVisited = DateTime.Now, Name = "PPHS", NumberOfStudents = 21}));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.SendAsync(request);           
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var createdTour = await response.Content.ReadAsAsync<ClassTour>();
            Assert.Equal("PPHS", createdTour.Name);
        }

        [Fact]
        public async void POST_classtour_invalidData_ShouldReturn400()
        {
            var tour = new ClassTour()
            {
                DayVisited = DateTime.Now,
                Name = "",
                NumberOfStudents = 23
            };

            var client = _factory.CreateClient();
            var user = await Utils.Login(client);
            var request = new HttpRequestMessage(HttpMethod.Post, "api/classtours");
            request.Headers.Add("Authorization", $"Bearer {user.Token}");
            request.Content = new StringContent(JsonConvert.SerializeObject(tour));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async void DELETE_classtour_validId_ShouldReturn200WithDeletedTour()
        {
            var tourId = 1;
            var client = _factory.CreateClient();
            var user = await Utils.Login(client);

            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/classtours/{tourId}");
            request.Headers.Add("Authorization", $"Bearer {user.Token}");
            var response = await client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var tour = await response.Content.ReadAsAsync<ClassTour>();
            Assert.Equal(tourId, tour.Id);
            Assert.True(tour.Deleted);
        }

        [Fact]
        public async void DELETE_classtour_invalidId_ShouldReturn404()
        {
            var tourId = 600;
            var client = _factory.CreateClient();
            var user = await Utils.Login(client);

            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/classtours/{tourId}");
            request.Headers.Add("Authorization", $"Bearer {user.Token}");
            var response = await client.SendAsync(request);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async void DELETE_classtour_usernotloggedin_ShouldReturn401()
        {
            var tourId = 600;
            var client = _factory.CreateClient();
            

            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/classtours/{tourId}");
            var response = await client.SendAsync(request);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }  
}
