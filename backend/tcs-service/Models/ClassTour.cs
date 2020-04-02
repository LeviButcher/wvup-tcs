using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tcs_service.Models {
    ///<summary>Represents the ClassTour table</summary>
    [Table ("ClassTours")]
    public class ClassTour {
        ///<summary>The unique Id of this classtour</summary>
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        ///<summary>The name of this classtour</summary>
        [Required]
        [MinLength (1)]
        public string Name { get; set; }

        ///<summary>The day the class tour visited the center</summary>
        [Required]
        public DateTime DayVisited { get; set; }

        ///<summary>The number of students in the class tour</summary>
        [Required]
        public int NumberOfStudents { get; set; }

        ///<summary>If the class tour has been deleted</summary>
        public bool Deleted { get; set; }
    }
}