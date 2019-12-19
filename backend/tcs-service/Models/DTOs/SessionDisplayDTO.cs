using System;
using System.Collections.Generic;

/*
    Should be used when Sessions are displayed in a table format or result set.
    Not the model to be used when creating sessions or updating them.
*/
namespace tcs_service.Models.DTOs
{
    public class SessionDisplayDTO
    {
        public int Id { get; set; }

        public int PersonId { get; set; }

        public Person Person { get; set; }

        public Semester Semester { get; set; }

        public int SemesterId { get; set; }

        public DateTime InTime { get; set; }

        public DateTime? OutTime { get; set; }

        public int SemesterCode { get; set; }

        public bool Tutoring { get; set; }

        public IEnumerable<Class> SelectedClasses { get; set; }

        public IEnumerable<Reason> SelectedReasons { get; set; }
    }
}