using System.Net;
using System.Net.Http;
using tcs_integration.test_utils;
using tcs_service;
using tcs_service.Helpers;
using tcs_service.Models;
using tcs_service.Models.DTOs;
using Xunit;

namespace tcs_integration.Controllers {
    [Collection ("Integration")]
    public class PersonControllerTest : IClassFixture<CustomWebApplicationFactory<Startup>> {

        private readonly CustomWebApplicationFactory<Startup> _factory;

        public PersonControllerTest (CustomWebApplicationFactory<Startup> factory) {
            _factory = factory;
        }

        [Theory]
        [InlineData ("lbutche3@wvup.edu")]
        [InlineData ("teacher@wvup.edu")]
        [InlineData ("991533860")] // student Id
        [InlineData ("771771771")] // teacher Id
        public async void GET_person_ShouldReturn200WithScheduleWhenStudent (string identifier) {
            var client = _factory.CreateClient ();

            var response = await client.GetAsync ($"api/person/{identifier}");
            Assert.Equal (HttpStatusCode.OK, response.StatusCode);
            var person = await response.Content.ReadAsAsync<PersonInfoDTO> ();
            Assert.NotNull (person);
            if (person.PersonType == PersonType.Student) {
                Assert.NotEmpty (person.Schedule);
            } else {
                Assert.Empty (person.Schedule);
            }
        }

        [Theory]
        [InlineData ("lbutche3@wvup.edu")]
        [InlineData ("teacher@wvup.edu")]
        [InlineData ("991533860")] // student Id
        [InlineData ("771771771")] // teacher Id
        public async void GET_person_admin_ShouldReturn200WithScheduleWhenStudent (string identifier) {
            var client = _factory.CreateClient ();

            var user = await Utils.Login (client);
            var request = new HttpRequestMessage (HttpMethod.Get, $"api/person/{identifier}/admin");
            request.Headers.Add ("Authorization", $"Bearer {user.Token}");

            var response = await client.SendAsync (request);
            Assert.Equal (HttpStatusCode.OK, response.StatusCode);
            var person = await response.Content.ReadAsAsync<PersonInfoDTO> ();
            Assert.NotNull (person);
            if (person.PersonType == PersonType.Student) {
                Assert.NotEmpty (person.Schedule);
            } else {
                Assert.Empty (person.Schedule);
            }
        }

        [Fact]
        public async void GET_person_PersonDoesNotExist_ShouldReturn500WithErrorMessage () {
            var email = "doesnotExist@wvup.edu";
            var client = _factory.CreateClient ();

            var response = await client.GetAsync ($"api/person/{email}");
            Assert.Equal (HttpStatusCode.InternalServerError, response.StatusCode);
            var error = await response.Content.ReadAsAsync<ErrorMessage> ();
            Assert.NotNull (error);
            Assert.NotNull (error.Message);
        }

        [Fact]
        public async void GET_person_admin_PersonDoesNotExist_ShouldReturn500WithErrorMessage () {
            var email = "doesnotExist@wvup.edu";
            var client = _factory.CreateClient ();

            var user = await Utils.Login (client);
            var request = new HttpRequestMessage (HttpMethod.Get, $"api/person/{email}/admin");
            request.Headers.Add ("Authorization", $"Bearer {user.Token}");

            var response = await client.SendAsync (request);
            Assert.Equal (HttpStatusCode.InternalServerError, response.StatusCode);
            var error = await response.Content.ReadAsAsync<ErrorMessage> ();
            Assert.NotNull (error);
            Assert.NotNull (error.Message);
        }
    }
}