using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tcs_service.Models.ViewModels
{
    public class TeacherInfoViewModel
    {
        public int teacherID { get; set; }

        public string teacherEmail { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }

        public int semesterId { get; set; }
    }
}
