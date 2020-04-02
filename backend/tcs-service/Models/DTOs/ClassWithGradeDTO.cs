namespace tcs_service.Models.DTO
{
    ///<summary>Reprents a Grade given in a specific Class</summary>
    public class ClassWithGradeDTO
    {
        ///<summary>The Department name of the class</summary>
        public string DepartmentName { get; set; }

        ///<summary>The CRN of the class</summary>
        public int CRN { get; set; }

        ///<summary>The CourseName of the class</summary>
        public string CourseName { get; set; }

        ///<summary>The MidtermGrade given for the class</summary>
        public Grade MidtermGrade { get; set; }

        ///<summary>The FinalGrade given for the class</summary>
        public Grade FinalGrade { get; set; }
    }
}

///<summary>The Different Grades possible to get</summary>
public enum Grade
{
    /// A
    A = 0,
    /// B
    B = 1,
    /// C
    C = 2,
    /// I
    I = 3,
    /// D
    D = 4,
    /// F
    F = 5,
    /// W
    W = 6,
    /// FIW
    FIW = 7
}