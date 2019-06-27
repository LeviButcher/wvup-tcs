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

        StudentInfoViewModel GetStudentInfoWithID(int studentID);

        StudentInfoViewModel GetStudentInfoWithEmail(string studentEmail);

        Task<Course> AddCourse(Course course);

        Task<Reason> AddReason(Reason reason);
    }
}
