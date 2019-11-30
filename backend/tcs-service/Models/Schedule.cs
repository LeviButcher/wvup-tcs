using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tcs_service.Models
{
    public class Schedule
    {
        [Key]
        public int PersonId { get; set; }

        public int ClassId { get; set; }

        public int SemesterId { get; set; }

    }
}
