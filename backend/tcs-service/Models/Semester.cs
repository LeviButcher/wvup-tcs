using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tcs_service.Models
{
    public class Semester
    {
        [Key]
        public int Code { get; set; }

        [NotMapped]
        public string Name { get; }
    }
}
