using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using tcs_service;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;
using tcs_service.Models;
using tcs_service.Models.DTOs;
using Newtonsoft.Json;
using tcs_integration.test_utils;

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
        [InlineData("991533860")]
        [InlineData("771771771")]
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
    }
}