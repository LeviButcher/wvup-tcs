using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tcs_service.Models
{
    public class Course
    {
        [Key]
        public int CRN { get; set; }

        [Required]
        public int DepartmentID { get; set; }

        [ForeignKey(nameof(DepartmentID))]
        public Department Department { get; set; }

        [Required]
        public string CourseName { get; set; }

        [Required]
        public string ShortName { get; set; }

        [InverseProperty(nameof(SignInCourse.Course))]
        public List<SignInCourse> SignInCourses { get; set; }
    }
}
