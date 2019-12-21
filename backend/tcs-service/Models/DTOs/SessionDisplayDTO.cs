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

        public PersonDisplayDTO Person { get; set; }

        public SemesterDisplayDTO Semester { get; set; }

        public DateTime InTime { get; set; }

        public DateTime? OutTime { get; set; }

        public int SemesterCode { get; set; }

        public bool Tutoring { get; set; }

        public IEnumerable<ClassDisplayDTO> SelectedClasses { get; set; }

        public IEnumerable<ReasonDisplayDTO> SelectedReasons { get; set; }
    }

    public class PersonDisplayDTO
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public PersonType PersonType { get; set; }

        public string FullName => FirstName + " " + LastName;
    }

    public class SemesterDisplayDTO
    {
        public int Code { get; set; }
        public string Name { get; set; }
    }

    public class ClassDisplayDTO
    {
        public int CRN { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public DepartmentDisplayDTO Department { get; set; }
    }

    public class DepartmentDisplayDTO
    {
        public string Name { get; set; }
        public int Code { get; set; }
    }

    public class ReasonDisplayDTO
    {
        public string Name { get; set; }
        public bool Deleted { get; set; }
        public int Id { get; set; }
    }
}