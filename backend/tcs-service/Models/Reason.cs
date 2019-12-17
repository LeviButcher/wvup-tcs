using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tcs_service.Models
{
    [Table("Reasons")]
    public class Reason
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public bool Deleted { get; set; }

        [InverseProperty(nameof(SessionReason.Reason))]
        public IEnumerable<SessionReason> SessionReasons { get; set; }
    }
}
