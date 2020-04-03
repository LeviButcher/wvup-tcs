using System.Collections.Generic;
using System.Threading.Tasks;
using tcs_service.Models.DTO;

namespace tcs_service.Services.Interfaces {
    ///<summary>Methods to interact with the Banner Api</summary>
    public interface IBannerService {

        ///<summary>Return back a person info using either their WVUP Id or Email</summary>
        /// Info is returned for both students and teachers
        /// Throws TCS Exception if no information is found
        Task<BannerPersonInfo> GetBannerInfo (string identifier);

        ///<summary>Return back the grade a student got in a specific class during a semester</summary>
        Task<ClassWithGradeDTO> GetStudentGrade (int studentId, int crn, int termCode);
    }
}

///<summary>The Data Banner API Returns from getting a Person Info</summary>
/// Properties match exact json that banner returns
public class BannerPersonInfo {

    ///<summary>The WVUP Id of the person</summary>
    public int WVUPID { get; set; }

    ///<summary>The email of the person</summary>
    public string EmailAddress { get; set; }

    ///<summary>The first name of the person</summary>
    public string FirstName { get; set; }

    ///<summary>The last name of the person</summary>
    public string LastName { get; set; }

    ///<summary>The semester code of the current semester</summary>
    public int TermCode { get; set; }

    ///<summary>If this person is a teacher</summary>
    public bool Teacher { get; set; }

    ///<summary>The current schedule of a student, only returned for students</summary>
    public IEnumerable<BannerClass> Courses { get; set; }
}

///<summary>The information about the Class Banner Returns</summary>
public class BannerClass {

    ///<summary>The CourseName Ex: Intro to Programming</summary>
    public string CourseName { get; set; }

    ///<summary>The ShortName Ex: CS121</summary>
    public string ShortName { get; set; }

    ///<summary>The Classes unique CRN</summary>
    public int CRN { get; set; }

    ///<summary>The Department information for this class</summary>
    public BannerDepartment Department { get; set; }
}
///<summary>The Department information returned by banner</summary>
public class BannerDepartment {
    ///<summary>The Name of the department</summary>
    public string Name { get; set; }

    ///<summary>The unique department code</summary>
    public int Code { get; set; }
}