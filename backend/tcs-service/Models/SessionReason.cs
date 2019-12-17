using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tcs_service.Models
{
    public class SessionReason
    {
        [Key]
        [Column(Order = 1)]
        [Required]
        public int ReasonId { get; set; }

        [ForeignKey(nameof(ReasonId))]
        public Reason Reason { get; set; }

        [Key]
        [Column(Order = 2)]
        [Required]
        public int SessionId { get; set; }

        [ForeignKey(nameof(SessionId))]
        public Session Session { get; set; }
    }
}