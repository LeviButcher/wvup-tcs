namespace tcs_service.Models.DTO
{
    ///<summary>Represents the Returned Banner Grade information</summary>
    public class BannerGradeInformationDTO
    {
        ///<summary>The CRN of the class this grade is for</summary>
        public int CRN { get; set; }

        ///<summary>The SubjectCode of the class this grade is for</summary>
        public string SubjectCode { get; set; }

        ///<summary>The CourseNumber of the class this grade is for</summary>
        public string CourseNumber { get; set; }

        ///<summary>The MidtermGrade for the class</summary>
        public string MidtermGrade { get; set; }

        ///<summary>The FinalGrade for the class</summary>
        public string FinalGrade { get; set; }

        ///<summary>The DepartmentName of the class this grade is for</summary>
        public string Department { get; set; }
    }
}
