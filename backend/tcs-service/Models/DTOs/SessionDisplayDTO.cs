using System;
using System.Collections.Generic;

/*
    Should be used when Sessions are displayed in a table format or result set.
    Not the model to be used when creating sessions or updating them.
*/
namespace tcs_service.Models.DTOs
{
    /// <summary>Represents displaying Session Data in a tabular format</summary>
    public class SessionDisplayDTO
    {
        /// <summary>The Id of the session</summary>
        public int Id { get; set; }

        /// <summary>The WVUP Id of the person who signed in</summary>
        public int PersonId { get; set; }

        /// <summary>The Person's Information</summary>
        public PersonDisplayDTO Person { get; set; }

        /// <summary>The Semesters Information</summary>
        public SemesterDisplayDTO Semester { get; set; }

        /// <summary>The time the session started</summary>
        public DateTime InTime { get; set; }

        /// <summary>The time the session ended</summary>
        public DateTime? OutTime { get; set; }

        /// <summary>The semesterCode of the semester session took place</summary>
        public int SemesterCode { get; set; }

        /// <summary>If the person was here for tutoring</summary>
        public bool Tutoring { get; set; }

        /// <summary>The classes that was selected if the person was a student</summary>
        public IEnumerable<ClassDisplayDTO> SelectedClasses { get; set; }

        /// <summary>The reasons that was selected if the person was a student</summary>
        public IEnumerable<ReasonDisplayDTO> SelectedReasons { get; set; }
    }

    /// <summary>The information about a Person</summary>
    public class PersonDisplayDTO
    {
        /// <summary>The WVUP Id of the person</summary>
        public int Id { get; set; }

        /// <summary>The Email of the person</summary>
        public string Email { get; set; }

        /// <summary>The FirstName of the person</summary>
        public string FirstName { get; set; }

        /// <summary>The LastName of the person</summary>
        public string LastName { get; set; }

        /// <summary>The PersonType of the person</summary>
        public PersonType PersonType { get; set; }

        /// <summary>The FullName of the person</summary>
        public string FullName => FirstName + " " + LastName;
    }

    /// <summary>The Semester Information</summary>
    public class SemesterDisplayDTO
    {
        /// <summary>The Semester Code</summary>
        public int Code { get; set; }

        /// <summary>The Semester Name</summary>
        public string Name { get; set; }
    }

    /// <summary>The Class Information</summary>
    public class ClassDisplayDTO
    {
        /// <summary>The CRN of the Class</summary>
        public int CRN { get; set; }

        /// <summary>The Name of the Class</summary>
        public string Name { get; set; }

        /// <summary>The ShortName of the Class</summary>
        public string ShortName { get; set; }

        /// <summary>The DepartmentName of the Class</summary>
        public DepartmentDisplayDTO Department { get; set; }
    }

    /// <summary>The Department Information</summary>
    public class DepartmentDisplayDTO
    {
        /// <summary>The Name of Department</summary>
        public string Name { get; set; }

        /// <summary>The Code of Department</summary>
        public int Code { get; set; }
    }

    /// <summary>The Reason Information</summary>
    public class ReasonDisplayDTO
    {
        /// <summary>The Name of the Reason</summary>
        public string Name { get; set; }

        /// <summary>Wether or not this reason has been deleted</summary>
        public bool Deleted { get; set; }

        /// <summary>The Id of the reason</summary>
        public int Id { get; set; }
    }
}