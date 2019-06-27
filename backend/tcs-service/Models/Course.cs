using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tcs_service.Models
{
    public class Course
    {
        [Key]
        [InverseProperty(nameof(SignInCourse.Course))]
        public int CRN { get; set; }

        [Required]
        public int DepartmentID { get; set; }

        [ForeignKey(nameof(DepartmentID))]
        public Department Department { get; set; }

        [Required]
        public string CourseName { get; set; }

        [Required]
        public string ShortName { get; set; }
    }
}
