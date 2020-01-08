namespace tcs_service.Models.DTO
{
    public class ClassWithGradeDTO
    {
        public string DepartmentName { get; set; }

        public int CRN { get; set; }

        public string CourseName { get; set; }

        public Grade Grade { get; set; }
    }
}

public enum Grade
{
    A = 0,
    B = 1,
    C = 2,
    I = 3,
    D = 4,
    F = 5,
    W = 6,
    FIW = 7
}