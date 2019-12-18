using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace tcs_service.Models.DTOs
{
    public class KioskSignInDTO
    {
        public int PersonId { get; set; }
        public bool Tutoring { get; set; }
        public IEnumerable<int> SelectedReasons { get; set; }

        public IEnumerable<int> SelectedClasses { get; set; }
    }
}