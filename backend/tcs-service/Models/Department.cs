using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tcs_service.Models
{
    public class Department
    {
        [Key]
        public int Code { get; set; }

        [Required]
        public string Name { get; set; }

        [InverseProperty(nameof(Course.Department))]
        public List<Course> Courses { get; set; }
    }
}
