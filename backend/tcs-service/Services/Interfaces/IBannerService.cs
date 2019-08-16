using System.Threading.Tasks;
using tcs_service.Models;
using tcs_service.Models.ViewModels;

namespace tcs_service.Services.Interfaces
{
    public interface IBannerService
    {
        Task<CourseWithGradeViewModel> GetStudentGrade(int studentId, int crn, int termCode);

        Task<StudentInfoViewModel> GetStudentInfoWithEmail(string studentEmail);

        Task<StudentInfoViewModel> GetStudentInfoWithID(int studentID);

        Task<TeacherInfoViewModel> GetTeacherInfoWithEmail(string teacherEmail);

        Task<TeacherInfoViewModel> GetTeacherInfoWithID(int teacherID);
    }
}
