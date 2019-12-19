using System.Collections.Generic;

namespace tcs_service.Models.DTOs
{
    public class SessionUpdateDTO : SessionCreateDTO
    {
        public PersonType PersonType { get; set; }

        public string Email { get; set; }

        public IEnumerable<Class> Schedule { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}