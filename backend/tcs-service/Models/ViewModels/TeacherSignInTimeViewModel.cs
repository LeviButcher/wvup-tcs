using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tcs_service.Models.ViewModels
{
    public class TeacherSignInTimeViewModel
    {
        public string teacherEmail { get; set; }

        public string fullName { get; set; }

        public decimal totalHours { get; set; }
    }
}
