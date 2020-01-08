using System;
using System.Net.Http;
using tcs_service.Models.DTO;
using tcs_service.Services.Interfaces;
using System.Threading.Tasks;
using tcs_service.Helpers;
using System.Net;

namespace tcs_service.Services
{
    public class LiveBannerService : IBannerService
    {
        readonly private HttpClient bannerApi;

        public LiveBannerService(IHttpClientFactory clientFactory)
        {
            bannerApi = clientFactory.CreateClient("banner");
        }

        public async Task<BannerPersonInfo> GetBannerInfo(string identifier)
        {
            var cleanedIdentifier = identifier.Split("@")[0];

            var studentResponse = await bannerApi.GetAsync($"student/{cleanedIdentifier}");
            var studentInfo = await studentResponse.Content.ReadAsAsync<BannerPersonInfo>();
            if (studentResponse.StatusCode == HttpStatusCode.OK) return studentInfo;


            var teacherResponse = await bannerApi.GetAsync($"teacher/{cleanedIdentifier}");
            var teacherInfo = await teacherResponse.Content.ReadAsAsync<BannerPersonInfo>();
            if (teacherResponse.StatusCode == HttpStatusCode.OK) return teacherInfo;

            if (studentResponse.StatusCode == HttpStatusCode.NotFound && teacherResponse.StatusCode == HttpStatusCode.NotFound)
                throw new TCSException($"Could not find information on: {identifier}");

            throw new TCSException("Banner is currently down");
        }

        public async Task<ClassWithGradeDTO> GetStudentGrade(int studentId, int crn, int termCode)
        {
            var response = await bannerApi.GetAsync($"student/{studentId}/{termCode}/{crn}");
            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new TCSException($"No information found for {studentId} during semester {termCode} for class {crn}");
            if (response.StatusCode != HttpStatusCode.OK) throw new TCSException("Banner is currently down");

            var bannerGradeInfo = await response.Content.ReadAsAsync<BannerGradeInformationDTO>();

            return new ClassWithGradeDTO()
            {
                CRN = bannerGradeInfo.CRN,
                CourseName = bannerGradeInfo.SubjectCode += bannerGradeInfo.CourseNumber,
                DepartmentName = bannerGradeInfo.SubjectCode,
                Grade = (Grade)Enum.Parse(typeof(Grade), bannerGradeInfo.FinalGrade)
            };
        }
    }
}
