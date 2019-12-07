using System;
using System.Net.Http;
using tcs_service.Models.ViewModels;
using tcs_service.Services.Interfaces;
using System.Threading.Tasks;
using AutoMapper;

namespace tcs_service.Services
{
    public class LiveBannerService : IBannerService
    {
        private readonly IMapper mapper;
        private HttpClient bannerApi;

        public LiveBannerService(IHttpClientFactory clientFactory, IMapper mapper)
        {
            bannerApi = clientFactory.CreateClient("banner");
            this.mapper = mapper;
        }

        public Task<BannerPersonInfo> GetBannerInfo(string identifier)
        {
            return null;
        }

        public async Task<CourseWithGradeViewModel> GetStudentGrade(int studentId, int crn, int termCode)
        {
            var response = await bannerApi.GetAsync($"student/{studentId}/{termCode}/{crn}");
            response.EnsureSuccessStatusCode();
            var bannerGradeInfo = await response.Content.ReadAsAsync<BannerGradeInformation>();

            return new CourseWithGradeViewModel()
            {
                CRN = bannerGradeInfo.CRN,
                CourseName = bannerGradeInfo.SubjectCode += bannerGradeInfo.CourseNumber,
                DepartmentName = bannerGradeInfo.SubjectCode,
                Grade = (Grade)Enum.Parse(typeof(Grade), bannerGradeInfo.FinalGrade)
            };
        }

        public async Task<StudentInfoViewModel> GetStudentInfoWithEmail(string studentEmail)
        {
            var emailStart = studentEmail.Split("@")[0];
            var response = await bannerApi.GetAsync($"student/{emailStart}");
            response.EnsureSuccessStatusCode();
            var bannerStudentInfo = await response.Content.ReadAsAsync<BannerInformation>();
            return mapper.Map<StudentInfoViewModel>(bannerStudentInfo);
        }

        public async Task<StudentInfoViewModel> GetStudentInfoWithID(int studentID)
        {
            var response = await bannerApi.GetAsync($"student/{studentID}");
            response.EnsureSuccessStatusCode();
            var bannerStudentInfo = await response.Content.ReadAsAsync<BannerInformation>();
            return mapper.Map<StudentInfoViewModel>(bannerStudentInfo);
        }

        public async Task<TeacherInfoViewModel> GetTeacherInfoWithEmail(string teacherEmail)
        {
            var emailStart = teacherEmail.Split("@")[0];
            var response = await bannerApi.GetAsync($"teacher/{emailStart}");
            response.EnsureSuccessStatusCode();
            var bannerStudentInfo = await response.Content.ReadAsAsync<BannerInformation>();
            return mapper.Map<TeacherInfoViewModel>(bannerStudentInfo);
        }

        public async Task<TeacherInfoViewModel> GetTeacherInfoWithID(int teacherID)
        {
            var response = await bannerApi.GetAsync($"teacher/{teacherID}");
            response.EnsureSuccessStatusCode();
            var bannerStudentInfo = await response.Content.ReadAsAsync<BannerInformation>();
            return mapper.Map<TeacherInfoViewModel>(bannerStudentInfo);
        }
    }
}
