using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tcs_service.Models;
using tcs_service.Models.ViewModels;
using tcs_service.Repos.Base;

namespace tcs_service.Repos.Interfaces
{
    public interface ISignInRepo : IRepo<SignIn>
    {
        Task<SignIn> Add(SignIn signIn);

        Task<SignIn> Update(SignIn signIn);

        Task<StudentInfoViewModel> GetStudentInfoWithID(int studentID);

        Task<StudentInfoViewModel> GetStudentInfoWithEmail(string studentEmail);

        Task<TeacherInfoViewModel> GetTeacherInfoWithID(int teacherID);

        Task<TeacherInfoViewModel> GetTeacherInfoWithEmail(string teacherEmail);

        Task<Course> AddCourse(Course course);

        Task<bool> CourseExist(int id);

        Task<Reason> AddReason(Reason reason);

        Task<bool> ReasonExist(int id);

        Task<Person> AddPerson(Person person);

        Task<bool> PersonExist(int id);

        Task<bool> PersonExist(string email);

        Task<bool> DepartmentExist(int id);

        Task<SignIn> GetMostRecentSignInByID(int id);

        Task<SignIn> GetMostRecentSignInByEmail(string email);

        Task<SignInViewModel> GetSignInViewModel(int id);

        void UpdateNullSignOuts();
    }
}
