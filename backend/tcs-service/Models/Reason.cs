using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tcs_service.Models
{
    public class Reason
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public bool Deleted { get; set; }

        [InverseProperty(nameof(SignInReason.Reason))]
        public List<SignInReason> SignInReasons { get; set; }
    }
}
