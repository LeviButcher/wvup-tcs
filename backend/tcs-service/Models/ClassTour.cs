using System;
using System.ComponentModel.DataAnnotations;

namespace tcs_service.Models
{
    public class ClassTour
    {
        [Key]
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
