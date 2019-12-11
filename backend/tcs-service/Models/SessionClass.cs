
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tcs_service.Models
{
    public class SessionClass
    {
        [Key]
        [Column(Order = 1)]
        [Required]
        public int ClassID { get; set; }

        [ForeignKey(nameof(ClassID))]
        public Class Class { get; set; }

        [Key]
        [Column(Order = 2)]
        [Required]
        public int SessionID { get; set; }


        [ForeignKey(nameof(SessionID))]
        public Session Session { get; set; }
    }
}
