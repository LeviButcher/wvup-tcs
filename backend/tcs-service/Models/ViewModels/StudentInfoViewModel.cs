using System.Collections.Generic;

namespace tcs_service.Models.ViewModels
{
    public class StudentInfoViewModel
    {
        public int studentID { get; set; }

        public string studentEmail { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }

        public List<Class> classSchedule { get; set; }

        public int semesterId { get; set; }

        public string personType { get; set; }
    }
}
