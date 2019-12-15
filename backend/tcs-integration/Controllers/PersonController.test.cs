using tcs_service;
using System.Net;
using System.Net.Http;
using Xunit;
using tcs_service.Models;
using tcs_service.Models.DTOs;
using tcs_integration.test_utils;

// Need to add in Admin api endpoints that sends back all Courses if student

namespace tcs_integration.Controllers
{
    [Collection("Integration")]
    public class PersonControllerTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {

        private readonly CustomWebApplicationFactory<Startup> _factory;

        public PersonControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("lbutche3@wvup.edu")]
        [InlineData("teacher@wvup.edu")]
        [InlineData("991533860")] // student Id
        [InlineData("771771771")] // teacher Id
        public async void GET_person_ShouldReturn200WithScheduleWhenStudent(string identifier)
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"api/person/{identifier}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var person = await response.Content.ReadAsAsync<PersonInfoDTO>();
            Assert.NotNull(person);
            if (person.PersonType == PersonType.Student)
            {
                Assert.NotEmpty(person.Schedule);
            }
            else
            {
                Assert.Empty(person.Schedule);
            }
        }

        [Theory]
        [InlineData("lbutche3@wvup.edu")]
        [InlineData("teacher@wvup.edu")]
        [InlineData("991533860")] // student Id
        [InlineData("771771771")] // teacher Id
        public async void GET_person_admin_ShouldReturn200WithScheduleWhenStudent(string identifier)
        {
            var client = _factory.CreateClient();

            var user = await Utils.Login(client);
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/person/{identifier}/admin");
            request.Headers.Add("Authorization", $"Bearer {user.Token}");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var person = await response.Content.ReadAsAsync<PersonInfoDTO>();
            Assert.NotNull(person);
            if (person.PersonType == PersonType.Student)
            {
                Assert.NotEmpty(person.Schedule);
            }
            else
            {
                Assert.Empty(person.Schedule);
            }
        }
    }
}