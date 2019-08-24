using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tcs_service.Models.ViewModels
{
    public class ClassTourViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MinLength(1)]
        public string Name { get; set; }

        [Required]
        public DateTime DayVisited { get; set; }

        [Required]
        public int NumberOfStudents { get; set; }
    }
}
