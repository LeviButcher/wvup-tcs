using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tcs_service.Models
{
    public class Person
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public PersonType PersonType { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
    }

    public enum PersonType
    {
        Student = 0,
        Teacher = 1
    }
}
