using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tcs_service.Models {
    ///<summary>Represents the Reason Table</summary>
    [Table ("Reasons")]
    public class Reason {

        ///<summary>The unique Id of the reason</summary>
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        ///<summary>The name of the reason</summary>
        [Required]
        public string Name { get; set; }

        ///<summary>if the reason has been deleted</summary>
        [Required]
        public bool Deleted { get; set; }

        ///<summary>The SessionReasons this reasons has been used in</summary>
        [InverseProperty (nameof (SessionReason.Reason))]
        public IEnumerable<SessionReason> SessionReasons { get; set; }
    }
}