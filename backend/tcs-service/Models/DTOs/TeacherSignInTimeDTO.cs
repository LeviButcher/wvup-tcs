using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tcs_service.Models.DTO
{
    public class TeacherSignInTimeDTO
    {
        public string TeacherEmail { get; set; }

        public string FullName { get; set; }

        public decimal TotalHours { get; set; }
    }
}
