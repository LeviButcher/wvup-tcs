using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using tcs_service.Models.DTOs;
using Xunit;

namespace tcs_integration.test_utils
{
    public class Utils
    {

        public static async Task<UserDto> Login(HttpClient client)
        {
            var logInResponse = await client.PostAsJsonAsync("api/users/authenticate", new { username = "tcs", password = "Develop@90" });
            Assert.Equal(HttpStatusCode.OK, logInResponse.StatusCode);
            var user = await logInResponse.Content.ReadAsAsync<UserDto>();
            return user;
        }
    }
}