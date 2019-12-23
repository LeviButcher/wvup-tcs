using System;
using System.Collections.Generic;

namespace tcs_service.Models.DTOs
{
    public class PersonInfoDTO
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public IEnumerable<Class> Schedule { get; set; } = new List<Class>();

        public PersonType PersonType { get; set; }
    }
}
