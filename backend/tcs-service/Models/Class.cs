using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tcs_service.Models
{
    public class Class
    {
        [Key]
        public int CRN { get; set; }

        [Required]
        public int DepartmentCode { get; set; }

        [ForeignKey(nameof(DepartmentCode))]
        public Department Department { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string ShortName { get; set; }

        [InverseProperty(nameof(SessionClass.Class))]
        public List<SessionClass> SessionClasses { get; set; }
    }
}
