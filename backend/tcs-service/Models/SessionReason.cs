using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tcs_service.Models {
    ///<summary>Represents the Session Reason table</summary>
    /// Mapping table of a M:M relationship
    public class SessionReason {
        ///<summary>The Id of the Reason chosen during signing in</summary>
        [Key]
        [Column (Order = 1)]
        [Required]
        public int ReasonId { get; set; }

        ///<summary>The actual Reason</summary>
        [ForeignKey (nameof (ReasonId))]
        public Reason Reason { get; set; }

        ///<summary>The Id of the session this reason was chosen for</summary>
        [Key]
        [Column (Order = 2)]
        [Required]
        public int SessionId { get; set; }

        ///<summary>The actual session</summary>
        [ForeignKey (nameof (SessionId))]
        public Session Session { get; set; }
    }
}