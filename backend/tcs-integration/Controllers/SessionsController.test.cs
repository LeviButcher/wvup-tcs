using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using tcs_integration.test_utils;
using tcs_service;
using tcs_service.Helpers;
using tcs_service.Models;
using tcs_service.Models.DTOs;
using Xunit;

namespace tcs_integration.Controllers {
    [Collection ("Integration")]
    public class SessionsControllerTest : IClassFixture<CustomWebApplicationFactory<Startup>> {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public SessionsControllerTest (CustomWebApplicationFactory<Startup> factory) {
            _factory = factory;
        }

        private async Task<UserDto> Login (HttpClient client) {
            var logInResponse = await client.PostAsJsonAsync ("api/users/authenticate", new { username = "tcs", password = "Develop@90" });
            Assert.Equal (HttpStatusCode.OK, logInResponse.StatusCode);
            var user = await logInResponse.Content.ReadAsAsync<UserDto> ();
            return user;
        }

        [Fact]
        public async void GET_Sessions_ShouldReturnListOfSessions () {
            var client = _factory.CreateClient ();
            var user = await Login (client);
            var request = new HttpRequestMessage (HttpMethod.Get, "api/sessions");
            request.Headers.Add ("Authorization", $"Bearer {user.Token}");

            var response = await client.SendAsync (request);
            Assert.Equal (HttpStatusCode.OK, response.StatusCode);
            var sessions = await response.Content.ReadAsAsync<Paging<Session>> ();

            Assert.NotNull (sessions);
        }

        [Fact]
        public async void GET_Sessions_Id_ShouldReturnSessionWithMatchingId () {
            var sessionId = 1;
            var client = _factory.CreateClient ();
            var user = await Login (client);
            var request = new HttpRequestMessage (HttpMethod.Get, $"api/sessions/{sessionId}");
            request.Headers.Add ("Authorization", $"Bearer {user.Token}");

            var response = await client.SendAsync (request);
            Assert.Equal (HttpStatusCode.OK, response.StatusCode);
            var session = await response.Content.ReadAsAsync<Session> ();

            Assert.Equal (sessionId, session.Id);
        }

        [Fact]
        public async void GET_Sessions_In_ShouldReturnListOfSessionsWhereOutTimeIsNull () {
            var client = _factory.CreateClient ();
            var user = await Login (client);
            var request = new HttpRequestMessage (HttpMethod.Get, "api/sessions/in");
            request.Headers.Add ("Authorization", $"Bearer {user.Token}");

            var response = await client.SendAsync (request);
            Assert.Equal (HttpStatusCode.OK, response.StatusCode);
            var sessions = await response.Content.ReadAsAsync<IEnumerable<Session>> ();

            Assert.All (sessions, s => Assert.Null (s.OutTime));
        }

        [Fact]
        public async void POST_Sessions_ShouldReturn201WithCreatedSession () {
            var client = _factory.CreateClient ();
            var session = new SessionPostOrPutDTO () {
                InTime = DateTime.Now,
                OutTime = DateTime.Now.Add (new TimeSpan (1)),
                PersonId = 1,
                Tutoring = true,
                SelectedClasses = new List<int> () { 3, 4 },
                SelectedReasons = new List<int> () { 2 },
                SemesterCode = 201901
            };
            var user = await Login (client);
            var request = new HttpRequestMessage (HttpMethod.Post, "api/sessions");
            request.Headers.Add ("Authorization", $"Bearer {user.Token}");
            request.Content = new StringContent (JsonConvert.SerializeObject (session));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue ("application/json");

            var response = await client.SendAsync (request);
            Assert.Equal (HttpStatusCode.Created, response.StatusCode);
            var createdSession = await response.Content.ReadAsAsync<Session> ();
            Assert.Equal (1, createdSession.PersonId);
            Assert.True (session.Tutoring);
        }

        [Fact]
        public async void POST_Sessions_TeacherSession_ShouldReturn201 () {
            var client = _factory.CreateClient ();
            var session = new SessionPostOrPutDTO () {
                InTime = DateTime.Now,
                OutTime = DateTime.Now.Add (new TimeSpan (1)),
                PersonId = 771771771,
                SemesterCode = 201901
            };
            var user = await Login (client);
            var request = new HttpRequestMessage (HttpMethod.Post, "api/sessions");
            request.Headers.Add ("Authorization", $"Bearer {user.Token}");
            request.Content = new StringContent (JsonConvert.SerializeObject (session));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue ("application/json");

            var response = await client.SendAsync (request);
            Assert.Equal (HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async void POST_Session_WithNoSelectedClasses_ShouldReturn500 () {
            var client = _factory.CreateClient ();
            var session = new SessionPostOrPutDTO () {
                InTime = DateTime.Now,
                OutTime = DateTime.Now.Add (new TimeSpan (1)),
                PersonId = 1,
                Tutoring = true,
                SelectedClasses = new List<int> () { },
                SelectedReasons = new List<int> () { 2 },
                SemesterCode = 201901
            };
            var user = await Login (client);
            var request = new HttpRequestMessage (HttpMethod.Post, "api/sessions");
            request.Headers.Add ("Authorization", $"Bearer {user.Token}");
            request.Content = new StringContent (JsonConvert.SerializeObject (session));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue ("application/json");

            var response = await client.SendAsync (request);
            Assert.Equal (HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async void POST_Session_WithNoSelectedReasons_TutoringTrue_ShouldReturn201WithCreatedSession () {
            var client = _factory.CreateClient ();
            var session = new SessionPostOrPutDTO () {
                InTime = DateTime.Now,
                OutTime = DateTime.Now.Add (new TimeSpan (1)),
                PersonId = 1,
                Tutoring = true,
                SelectedClasses = new List<int> () { 2 },
                SelectedReasons = new List<int> () { },
                SemesterCode = 201901
            };
            var user = await Login (client);
            var request = new HttpRequestMessage (HttpMethod.Post, "api/sessions");
            request.Headers.Add ("Authorization", $"Bearer {user.Token}");
            request.Content = new StringContent (JsonConvert.SerializeObject (session));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue ("application/json");

            var response = await client.SendAsync (request);
            Assert.Equal (HttpStatusCode.Created, response.StatusCode);
            var createdSession = await response.Content.ReadAsAsync<Session> ();
            Assert.Equal (1, createdSession.PersonId);
            Assert.True (session.Tutoring);
        }

        [Fact]
        public async void POST_Session_WithNoSelectedReasons_TutoringFalse_ShouldReturn500 () {
            var client = _factory.CreateClient ();
            var session = new SessionPostOrPutDTO () {
                InTime = DateTime.Now,
                OutTime = DateTime.Now.Add (new TimeSpan (1)),
                PersonId = 1,
                Tutoring = false,
                SelectedClasses = new List<int> () { 3 },
                SelectedReasons = new List<int> () { },
                SemesterCode = 201901
            };
            var user = await Login (client);
            var request = new HttpRequestMessage (HttpMethod.Post, "api/sessions");
            request.Headers.Add ("Authorization", $"Bearer {user.Token}");
            request.Content = new StringContent (JsonConvert.SerializeObject (session));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue ("application/json");

            var response = await client.SendAsync (request);
            Assert.Equal (HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async void PUT_Sessions_Id_ShouldReturn200WithUpdatedSession () {
            var client = _factory.CreateClient ();
            var user = await Login (client);
            var request = new HttpRequestMessage (HttpMethod.Put, "api/sessions/3");
            var sessionDTO = new SessionPostOrPutDTO () {
                Id = 3,
                InTime = DateTime.Now,
                OutTime = DateTime.Now.Add (new TimeSpan (1)),
                PersonId = 8,
                Tutoring = true,
                SelectedClasses = new List<int> () { 3 },
                SelectedReasons = new List<int> () { },
                SemesterCode = 201901
            };
            request.Headers.Add ("Authorization", $"Bearer {user.Token}");
            request.Content = new StringContent (JsonConvert.SerializeObject (sessionDTO));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue ("application/json");

            var response = await client.SendAsync (request);
            Assert.Equal (HttpStatusCode.OK, response.StatusCode);
            var session = await response.Content.ReadAsAsync<Session> ();
            Assert.Equal (8, session.PersonId);
        }

        [Fact]
        public async void PUT_Session_Out_StudentSignedIn_ShouldReturn200WithSignedOutSession () {
            var client = _factory.CreateClient ();
            var signOut = new KioskSignOutDTO () {
                PersonId = 61
            };
            var response = await client.PutAsJsonAsync ("api/sessions/out", signOut);

            Assert.Equal (HttpStatusCode.OK, response.StatusCode);
            var session = await response.Content.ReadAsAsync<Session> ();
            Assert.NotNull (session.OutTime);
        }

        [Fact]
        public async void PUT_Sessions_Out_StudentNotSignedIn_ShouldReturn500 () {
            var signOut = new KioskSignOutDTO () {
                PersonId = 35
            };

            var client = _factory.CreateClient ();
            var response = await client.PutAsJsonAsync ("api/sessions/out", signOut);
            Assert.Equal (HttpStatusCode.InternalServerError, response.StatusCode);
            var errorMessage = await response.Content.ReadAsAsync<ErrorMessage> ();
            Assert.Equal ("You aren't signed in.", errorMessage.Message);
        }

        [Fact]
        public async void DELETE_Sessions_Id_ShouldReturn200WithDeletedSession () {
            var client = _factory.CreateClient ();
            var user = await Login (client);
            var request = new HttpRequestMessage (HttpMethod.Delete, "api/sessions/27");
            request.Headers.Add ("Authorization", $"Bearer {user.Token}");

            var response = await client.SendAsync (request);
            Assert.Equal (HttpStatusCode.OK, response.StatusCode);
            var session = await response.Content.ReadAsAsync<Session> ();
            Assert.Equal (27, session.Id);
            Assert.True (session.Deleted);
        }

        [Fact]
        public async void POST_Sessions_In_StudentNotSignedIn_ShouldReturn201 () {
            var session = new KioskSignInDTO () {
                PersonId = 4,
                Tutoring = false,
                SelectedClasses = new List<int> () { 1, 2 },
                SelectedReasons = new List<int> () { 1 }
            };

            var client = _factory.CreateClient ();
            var response = await client.PostAsJsonAsync ("api/sessions/in", session);
            Assert.Equal (HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async void POST_Sessions_In_StudentAlreadySignedIn_ShouldReturn500WithMessage () {
            var session = new KioskSignInDTO () {
                PersonId = 2,
                Tutoring = false,
                SelectedClasses = new List<int> () { 1, 2 },
                SelectedReasons = new List<int> () { 1 }
            };

            var client = _factory.CreateClient ();

            await client.PostAsJsonAsync ("api/sessions/in", session);
            var response = await client.PostAsJsonAsync ("api/sessions/in", session);
            Assert.Equal (HttpStatusCode.InternalServerError, response.StatusCode);
            var errorMessage = await response.Content.ReadAsAsync<ErrorMessage> ();
            Assert.Equal ("You are already signed in.", errorMessage.Message);
        }

        [Fact]
        public async void POST_Sessions_In_StudentSelectsNoReasonsWithTutoringTrue_ShouldReturn201 () {
            var session = new KioskSignInDTO () {
                PersonId = 7,
                Tutoring = true,
                SelectedClasses = new List<int> () { 1, 2 },
                SelectedReasons = new List<int> () { }
            };

            var client = _factory.CreateClient ();

            var response = await client.PostAsJsonAsync ("api/sessions/in", session);
            Assert.Equal (HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async void POST_Session_In_StudentSelectsNoReasonsWithTutoringFalse_ShouldReturn500WithMessage () {
            var session = new KioskSignInDTO () {
                PersonId = 7,
                Tutoring = false,
                SelectedClasses = new List<int> () { 1, 2 },
                SelectedReasons = new List<int> () { }
            };

            var client = _factory.CreateClient ();

            var response = await client.PostAsJsonAsync ("api/sessions/in", session);
            Assert.Equal (HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async void POST_Session_In_TeacherSignsIn_ShouldReturn201 () {
            var session = new KioskSignInDTO () {
                PersonId = 771771771
            };

            var client = _factory.CreateClient ();

            var response = await client.PostAsJsonAsync ("api/sessions/in", session);
            Assert.Equal (HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async void GET_Sessions_StartEqualToday_ShouldReturn200WithPageOfSessionsOnOrAfterToday () {
            var startDate = new DateTime (2019, 5, 1).Date;
            var client = _factory.CreateClient ();
            var user = await Login (client);
            var request = new HttpRequestMessage (HttpMethod.Get, $"api/sessions?start={startDate}");
            request.Headers.Add ("Authorization", $"Bearer {user.Token}");

            var response = await client.SendAsync (request);
            Assert.Equal (HttpStatusCode.OK, response.StatusCode);
            var page = await response.Content.ReadAsAsync<Paging<SessionDisplayDTO>> ();
            Assert.All (page.Data, x => Assert.True (startDate <= x.InTime.Date));
        }

        [Fact]
        public async void GET_Sessions_EndEqualToday_ShouldReturn200WithPageOfSessionsBeforeOrOnEndDate () {
            var endDate = new DateTime (2019, 7, 1).Date;
            var client = _factory.CreateClient ();
            var user = await Login (client);
            var request = new HttpRequestMessage (HttpMethod.Get, $"api/sessions?end={endDate}");
            request.Headers.Add ("Authorization", $"Bearer {user.Token}");

            var response = await client.SendAsync (request);
            Assert.Equal (HttpStatusCode.OK, response.StatusCode);
            var page = await response.Content.ReadAsAsync<Paging<SessionDisplayDTO>> ();
            Assert.All (page.Data, x => Assert.True (endDate >= x.InTime.Date));
        }

        [Fact]
        public async void GET_Sessions_CRNIsGiven_ShouldReturn200WithSessionsOnlyForThatClass () {
            var crn = 1;
            var client = _factory.CreateClient ();
            var user = await Login (client);
            var request = new HttpRequestMessage (HttpMethod.Get, $"api/sessions?crn={crn}");
            request.Headers.Add ("Authorization", $"Bearer {user.Token}");

            var response = await client.SendAsync (request);
            Assert.Equal (HttpStatusCode.OK, response.StatusCode);
            var page = await response.Content.ReadAsAsync<Paging<SessionDisplayDTO>> ();
            Assert.All (page.Data, x => Assert.Contains (x.SelectedClasses, a => a.CRN == crn));
        }

        [Fact]
        public async void GET_Sessions_EmailIsGiven_ShouldReturn200WithSessionOnlyForThePersonWithThatEmail () {
            var email = "lbutche3@wvup.edu";
            var client = _factory.CreateClient ();
            var user = await Login (client);
            var request = new HttpRequestMessage (HttpMethod.Get, $"api/sessions?email={email}");
            request.Headers.Add ("Authorization", $"Bearer {user.Token}");

            var response = await client.SendAsync (request);
            Assert.Equal (HttpStatusCode.OK, response.StatusCode);
            var page = await response.Content.ReadAsAsync<Paging<SessionDisplayDTO>> ();
            Assert.All (page.Data, x => Assert.Equal (x.Person.Email, email));
        }

        [Fact]
        public async void GET_Sessions_Semester_ShouldReturn200WithSessionsOnlyForThatSemester () {
            var semesterCode = 201902;
            var client = _factory.CreateClient ();
            var user = await Login (client);
            var request = new HttpRequestMessage (HttpMethod.Get, $"api/sessions/semester/{semesterCode}");
            request.Headers.Add ("Authorization", $"Bearer {user.Token}");

            var response = await client.SendAsync (request);
            Assert.Equal (HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadAsAsync<IEnumerable<SessionDisplayDTO>> ();
            Assert.All (result, x => Assert.Equal (semesterCode, x.SemesterCode));
        }

        [Fact]
        public async void POST_Sessions_Upload_ShouldReturn200 () {
            var filePath = "./test-utils/FakeData.csv";
            HttpContent fileStream = new StreamContent (File.OpenRead (filePath));

            var client = _factory.CreateClient ();
            var user = await Login (client);
            var data = new MultipartFormDataContent { { fileStream, "csvFile", "csvFile" }
            };
            var request = new HttpRequestMessage (HttpMethod.Post, "api/sessions/upload") {
                Content = data
            };
            request.Headers.Add ("Authorization", $"Bearer {user.Token}");

            var response = await client.SendAsync (request);
            Assert.Equal (HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async void POST_Sessions_Upload_BadCSVFile_ShouldReturn400 () {
            var filePath = "./test-utils/BadFakeData.csv";
            HttpContent fileStream = new StreamContent (File.OpenRead (filePath));

            var client = _factory.CreateClient ();
            var user = await Login (client);
            var data = new MultipartFormDataContent { { fileStream, "csvFile", "csvFile" }
            };
            var request = new HttpRequestMessage (HttpMethod.Post, "api/sessions/upload") {
                Content = data
            };
            request.Headers.Add ("Authorization", $"Bearer {user.Token}");

            var response = await client.SendAsync (request);
            Assert.Equal (HttpStatusCode.InternalServerError, response.StatusCode);
            var error = await response.Content.ReadAsAsync<ErrorMessage> ();
            Assert.NotNull (error.Message);
        }
    }
}