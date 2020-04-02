using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tcs_service.Models {
    ///<summary>Represents a person table</summary>
    /// A person could be a Teacher or a Student based on PersonType
    public class Person {

        ///<summary>The WVUP Id of the person</summary>
        [Key]
        public int Id { get; set; }

        ///<summary>The Email of the person</summary>
        [Required]
        public string Email { get; set; }

        ///<summary>The Type of the person</summary>
        [Required]
        public PersonType PersonType { get; set; }

        ///<summary>The FirstName of the person</summary>
        [Required]
        public string FirstName { get; set; }

        ///<summary>The LastName of the person</summary>
        [Required]
        public string LastName { get; set; }

        ///<summary>The Schedule of the person if they are a student</summary>
        [InverseProperty (nameof (Schedule.Person))]
        public IEnumerable<Schedule> Schedules { get; set; }
    }

    ///<summary>Represents the different type of People</summary>
    public enum PersonType {
        /// The Person is a Student
        Student = 0,
        /// The Person is a Teacher
        Teacher = 1
    }
}