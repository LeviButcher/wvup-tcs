namespace tcs_service.Models.DTO
{
    ///<summary>Represents the Return Data used for the Success Report</summary>
    /// <remarks>Keeps track wether a given grade in a class was Successful, Unique, Passed, Or Dropped</remarks>
    public class ClassSuccessCountDTO
    {
        ///<summary>The CRN of the Class</summary>
        public int CRN { get; set; }

        ///<summary>The ClassName of the Class</summary>
        public string ClassName { get; set; }

        ///<summary>The DepartmentName of the Class</summary>
        public string DepartmentName { get; set; }

        ///<summary>A Count of the Unique Students in a class</summary>
        public int UniqueStudentCount { get; set; } = 0;

        ///<summary>The amount of students who dropped a class, Grade of FIW or W</summary>
        public int DroppedStudentCount { get; set; } = 0;

        ///<summary>The amount of students who completed a class, Grade of A,B,C,I,D, or F</summary>
        public int CompletedCourseCount { get; set; } = 0;

        ///<summary>The amount of students who passed a class, Grade of A,B,C,I</summary>
        public int PassedSuccessfullyCount { get; set; } = 0;

        ///<summary>Construct a ClassSuccessCountDTO, all counts set to 0</summary>
        public ClassSuccessCountDTO(int crn, string className, string departmentName)
        {
            this.CRN = crn;
            this.ClassName = className;
            this.DepartmentName = departmentName;
        }
        private ClassSuccessCountDTO()
        { }

        /// <summary>
        /// Returns a new ClassSuccessCountDTO with the different counts
        /// updated depending on the grade
        /// </summary>
        /// <remarks>
        /// Here is the rules of updating the differrent counts:
        /// Passed: A,B,C,I
        /// Completed: A,B,C,I,D,F
        /// Dropped: FIW,W
        /// Unique: All Possible Grades
        /// </remarks>
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
