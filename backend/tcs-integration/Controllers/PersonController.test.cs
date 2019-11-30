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
    public class PersonControllerTest : IClassFixture<WebApplicationFactory<Startup>>
    {

        private readonly WebApplicationFactory<Startup> _factory;

        public PersonControllerTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async void GET_person_email_ShouldReturn200WithScheduleWhenStudent()
        {
            var email = "lbutche3@wvup.edu";
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"api/person/{email}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var person = await response.Content.ReadAsAsync<Person>();
            Assert.NotNull(person);
            Assert.NotNull(person.Schedule);
        }
    }
}