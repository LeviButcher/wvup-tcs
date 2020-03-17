namespace tcs_service.Models.DTO
{
    public class ClassSuccessCountDTO
    {
        public int CRN { get; set; }

        public string ClassName { get; set; }

        public string DepartmentName { get; set; }

        public int UniqueStudentCount { get; set; } = 0;

        public int DroppedStudentCount { get; set; } = 0;

        public int CompletedCourseCount { get; set; } = 0;

        public int PassedSuccessfullyCount { get; set; } = 0;

        public ClassSuccessCountDTO(int crn, string className, string departmentName)
        {
            this.CRN = crn;
            this.ClassName = className;
            this.DepartmentName = departmentName;
        }
        public ClassSuccessCountDTO()
        { }


        public ClassSuccessCountDTO DetermineSuccess(Grade grade)
        {
            int passed = this.PassedSuccessfullyCount;
            int completed = this.CompletedCourseCount;
            int dropped = this.DroppedStudentCount;
            int unique = this.UniqueStudentCount + 1;

            switch (grade)
            {
                case Grade.A:
                case Grade.B:
                case Grade.C:
                case Grade.I:
                    passed++;
                    completed++;
                    break;
                case Grade.D:
                case Grade.F:
                    completed++;
                    break;
                case Grade.FIW:
                case Grade.W:
                    dropped++;
                    break;
            }
            return new ClassSuccessCountDTO()
            {
                CRN = this.CRN,
                ClassName = this.ClassName,
                DepartmentName = this.DepartmentName,
                PassedSuccessfullyCount = passed,
                CompletedCourseCount = completed,
                UniqueStudentCount = unique,
                DroppedStudentCount = dropped
            };
        }
    }
}
