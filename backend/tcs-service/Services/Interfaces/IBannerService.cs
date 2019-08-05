using tcs_service.Models;
using tcs_service.Models.ViewModels;

namespace tcs_service.Services.Interfaces
{
    public interface IBannerService
    {
        CourseWithGradeViewModel GetStudentGrade(int studentId, Course course, Department department);

        StudentInfoViewModel GetStudentInfoWithEmail(string studentEmail);

        StudentInfoViewModel GetStudentInfoWithID(int studentID);

        TeacherInfoViewModel GetTeacherInfoWithEmail(string teacherEmail);

        TeacherInfoViewModel GetTeacherInfoWithID(int teacherID);

        
    }
}
