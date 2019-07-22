using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tcs_service.Models.ViewModels
{
    public class CourseWithGradeViewModel
    {
        public string DepartmentName { get; set; }

        public int CRN { get; set; }

        public string CourseName { get; set; }

        public Grade Grade { get; set; }
    }
}

public enum Grade
{
    A=1,
    B=2,
    C=3,
    I=4,
    D=5,
    F=6,
    W=7,
    FIW=8
}