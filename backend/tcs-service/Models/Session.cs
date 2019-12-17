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
        public int Id { get; set; }

        [Required]
        public int PersonId { get; set; }

        [ForeignKey(nameof(PersonId))]
        public Person Person { get; set; }

        private DateTimeOffset? inTime;

        [Required]
        public DateTimeOffset? InTime
        {
            get
            {
                if (!inTime.HasValue) return null;
                return inTime.Value.AddTicks(-inTime.Value.Ticks % TimeSpan.TicksPerSecond);
            }
            set => inTime = value;
        }

        private DateTimeOffset? outTime;

        [SignOutValidation("InTime")]
        public DateTimeOffset? OutTime
        {
            get
            {
                if (!outTime.HasValue) return null;
                return outTime.Value.AddTicks(-outTime.Value.Ticks % TimeSpan.TicksPerSecond);
            }
            set => outTime = value;
        }

        public bool Tutoring { get; set; }

        [InverseProperty(nameof(SessionClass.Session))]
        public List<SessionClass> SessionClasses { get; set; } = new List<SessionClass>();

        [InverseProperty(nameof(SessionReason.Session))]
        public List<SessionReason> SessionReasons { get; set; } = new List<SessionReason>();


        public int SemesterCode { get; set; }

        [ForeignKey(nameof(SemesterCode))]
        public Semester Semester { get; set; }

    }
}