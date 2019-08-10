using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tcs_service.Models
{
    public class SignInReason
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
        public int SignInID { get; set; }

        [ForeignKey(nameof(SignInID))]
        public SignIn SignIn { get; set; }
    }
}
