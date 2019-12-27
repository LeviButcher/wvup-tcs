using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tcs_service.Models.DTO
{
    public class ClassTourReportDTO
    {
        public string Name { get; set; }

        public int Students { get; set; }

        public DateTime DayVisited { get; set; }
    }
}
