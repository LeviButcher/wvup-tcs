using System.ComponentModel.DataAnnotations.Schema;

namespace tcs_service.Models
{
    public class Schedule
    {
        public int PersonId { get; set; }

        [ForeignKey(nameof(PersonId))]
        public Person Person { get; set; }

        public int ClassCRN { get; set; }

        [ForeignKey(nameof(ClassCRN))]
        public Class Class { get; set; }

        public int SemesterCode { get; set; }

        [ForeignKey(nameof(SemesterCode))]
        public Semester Semester { get; set; }

    }
}
