using System.ComponentModel.DataAnnotations;

namespace tcs_service.Models
{
    public class Semester
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
