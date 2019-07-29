using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tcs_service.Models
{
    public class SignIn
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int PersonId { get; set; }

        [ForeignKey(nameof(PersonId))]
        public Person Person { get; set; }

        [Required]
        public int SemesterId { get; set; }

        [ForeignKey(nameof(SemesterId))]
        public Semester Semester { get; set; }

        private DateTime? inTime;

        [Required]
        public DateTime? InTime
        {
            get
            {
                if (!inTime.HasValue) return null;
                return inTime.Value.AddTicks(-inTime.Value.Ticks % TimeSpan.TicksPerSecond);
            }
            set => inTime = value;
        }

        private DateTime? outTime;
        public DateTime? OutTime
        {
            get
            {
                if (!outTime.HasValue) return null;
                return outTime.Value.AddTicks(-outTime.Value.Ticks % TimeSpan.TicksPerSecond);
            }
            set => outTime = value;
        }

        public bool Tutoring { get; set; }

        [InverseProperty(nameof(SignInCourse.SignIn))]
        public List<SignInCourse> Courses { get; set; } = new List<SignInCourse>();

        [InverseProperty(nameof(SignInReason.SignIn))]
        public List<SignInReason> Reasons { get; set; } = new List<SignInReason>();
    }
}
