using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using tcs_service.Models.Attributes;

namespace tcs_service.Models
{
    public class Session
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int PersonId { get; set; }

        [ForeignKey(nameof(PersonId))]
        public Person Person { get; set; }


        [Required]
        public DateTime InTime { get; set; }


        [SignOutValidation("InTime")]
        public DateTime? OutTime { get; set; }


        public bool Tutoring { get; set; }

        [InverseProperty(nameof(SessionClass.Session))]
        public IEnumerable<SessionClass> SessionClasses { get; set; } = new List<SessionClass>();

        [InverseProperty(nameof(SessionReason.Session))]
        public IEnumerable<SessionReason> SessionReasons { get; set; } = new List<SessionReason>();


        public int SemesterCode { get; set; }

        [ForeignKey(nameof(SemesterCode))]
        public Semester Semester { get; set; }

        public bool Deleted { get; set; }

    }
}