using System.Collections.Generic;

namespace tcs_service.Models.DTO
{
    public class StudentInfoDTO
    {
        public int StudentId { get; set; }

        public string StudentEmail { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public List<Class> ClassSchedule { get; set; }

        public int SemesterId { get; set; }

        public string PersonType { get; set; }
    }
}
