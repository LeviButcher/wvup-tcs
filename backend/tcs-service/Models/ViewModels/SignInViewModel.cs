using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using tcs_service.Models.Attributes;

namespace tcs_service.Models.ViewModels
{
    public class SignInViewModel
    {
        public int Id { get; set; }

        public int PersonId { get; set; }

        public PersonType Type { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public int SemesterId { get; set; }

        public string SemesterName { get; set; }

        [DataType(DataType.DateTime)]
        public DateTimeOffset? InTime { get; set; }

        [DataType(DataType.DateTime)]
        [SignOutValidation("InTime")]
        public DateTimeOffset? OutTime { get; set; }

        public bool Tutoring { get; set; }

        public List<Class> Classes { get; set; }

        public List<Reason> Reasons { get; set; }
    }
}
