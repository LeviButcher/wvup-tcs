using System.Collections.Generic;
using System.Threading.Tasks;
using tcs_service.Models.ViewModels;

namespace tcs_service.Services.Interfaces
{
    public interface IBannerService
    {
        Task<BannerPersonInfo> GetBannerInfo(string identifier);
        Task<CourseWithGradeViewModel> GetStudentGrade(int studentId, int crn, int termCode);
    }
}

public class BannerPersonInfo
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int SemesterId { get; set; }
    public bool Teacher { get; set; }
    public IEnumerable<BannerCourse> Courses { get; set; }
}


public class BannerCourse
{
    public string CourseName { get; set; }
    public string ShortName { get; set; }
    public int CRN { get; set; }
    public BannerDepartment Department { get; set; }
}

public class BannerDepartment
{
    public string Name { get; set; }
    public int Code { get; set; }
}