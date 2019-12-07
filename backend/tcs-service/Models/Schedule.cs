using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tcs_service.Models
{
    public class Schedule
    {
        [Key]
        public int PersonId { get; set; }

        [ForeignKey(nameof(PersonId))]
        public Person Person { get; set; }

        public int CourseCRN { get; set; }

        [ForeignKey(nameof(CourseCRN))]
        public Course Course { get; set; }

        public int SemesterCode { get; set; }

        [ForeignKey(nameof(SemesterCode))]
        public Semester Semester { get; set; }

    }
}
