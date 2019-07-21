using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tcs_service.Models.ViewModels
{
    public class CourseWithSuccessCountViewModel
    {
        public int CRN { get; set; }

        public string ClassName { get; set; }

        public string DepartmentName {get;set;}

        public int UniqueStudentCount { get; set; }

        public int DroppedStudentCount { get; set; }

        public int CompletedCourseCount { get; set; }

        public int PassedSuccessfullyCount { get; set; }
    }
}
