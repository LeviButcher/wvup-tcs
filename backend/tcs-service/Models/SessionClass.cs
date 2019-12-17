
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tcs_service.Models
{
    public class SessionClass
    {
        [Key]
        [Column(Order = 1)]
        [Required]
        public int ClassId { get; set; }

        [ForeignKey(nameof(ClassId))]
        public Class Class { get; set; }

        [Key]
        [Column(Order = 2)]
        [Required]
        public int SessionId { get; set; }


        [ForeignKey(nameof(SessionId))]
        public Session Session { get; set; }
    }
}
