using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tcs_service.Models
{
    public class SignInCourse
    {
        [Required]
        public int CourseID { get; set; }

        [ForeignKey(nameof(CourseID))]
        public Course Course { get; set; }

        [Required]
        public int SignInID { get; set; }

        [ForeignKey(nameof(SignInID))]
        public SignIn SignIn { get; set; }
    }
}
