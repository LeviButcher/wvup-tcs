using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tcs_service.Models.ViewModels
{
    public class ReasonWithClassVisitsViewModel
    {
        public string reasonName { get; set; }

        public int reasonId { get; set; }

        public string courseName { get; set; }

        public int courseCRN { get; set; }

        public int visits { get; set; }
    }
}
