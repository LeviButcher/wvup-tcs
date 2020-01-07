using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tcs_service.Models.DTO
{
    public class ReasonWithClassVisitsDTO
    {
        public string ReasonName { get; set; }

        public int ReasonId { get; set; }

        public string ClassName { get; set; }

        public int ClassCRN { get; set; }

        public int Visits { get; set; }
    }
}
