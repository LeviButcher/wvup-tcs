using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tcs_service.Models
{
    public class SessionReason
    {
        [Key]
        [Column(Order = 1)]
        [Required]
        public int ReasonID { get; set; }

        [ForeignKey(nameof(ReasonID))]
        public Reason Reason { get; set; }

        [Key]
        [Column(Order = 2)]
        [Required]
        public int SessionID { get; set; }

        [ForeignKey(nameof(SessionID))]
        public Session Session { get; set; }
    }
}