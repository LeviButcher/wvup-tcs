using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using tcs_service.Models.Attributes;

namespace tcs_service.Models.DTOs
{
    public class SessionInfoDTO : SessionPostOrPutDTO
    {
        [DataType(DataType.DateTime)]
        [SignOutValidation("InTime")]
        new public DateTime? OutTime { get; set; }

        public PersonType PersonType { get; set; }

        public string Email { get; set; }

        public IEnumerable<Class> Schedule { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}