using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tcs_service.Models
{
    [Table("ClassTours")]
    public class ClassTour
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MinLength(1)]
        public string Name { get; set; }

        [Required]
        public DateTime DayVisited { get; set; }

        [Required]
        public int NumberOfStudents { get; set; }

        public bool Deleted { get; set; }
    }
}
