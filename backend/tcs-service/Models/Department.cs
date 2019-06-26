using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tcs_service.Models
{
    public class Department
    {
        [Key]
        public int Code { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
