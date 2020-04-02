using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tcs_service.Models {
    /// <summary>Represents the SessionClass table</summary>
    /// Mapping table of a M:M relationship
    public class SessionClass {

        /// <summary>The class CRN of the class that was chosen for a session</summary>
        [Key]
        [Column (Order = 1)]
        [Required]
        public int ClassId { get; set; }

        /// <summary>The actual Class</summary>
        [ForeignKey (nameof (ClassId))]
        public Class Class { get; set; }

        /// <summary>The session id that this class was chosen for</summary>
        [Key]
        [Column (Order = 2)]
        [Required]
        public int SessionId { get; set; }

        /// <summary>The actual Session</summary>
        [ForeignKey (nameof (SessionId))]
        public Session Session { get; set; }
    }
}