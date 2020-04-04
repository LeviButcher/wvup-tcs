using System.Collections.Generic;

namespace tcs_service.Models.DTOs
{
    /// <summary>Represents a Submitting of someone signing in at the kiosk</summary>
    public class KioskSignInDTO
    {
        /// <summary>The WVUPID of the person signing in</summary>
        public int PersonId { get; set; }

        /// <summary>Wether or not the person selected tutoring</summary>
        public bool Tutoring { get; set; }

        /// <summary>The Id's of the Reasons the Person selected</summary>
        public IEnumerable<int> SelectedReasons { get; set; }

        /// <summary>The CRNs of the Classes the Person selected</summary>
        public IEnumerable<int> SelectedClasses { get; set; }
    }
}