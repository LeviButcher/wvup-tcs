using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tcs_service.Models.ViewModels
{
    public class CourseWithGradeViewModel
    {
        public string Department { get; set; }

        public int CRN { get; set; }

        public string CourseName { get; set; }

        public string Grade { get; set; }
    }
}
