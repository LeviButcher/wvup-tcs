using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tcs_service.Models {
    ///<summary>Represents the Class table</summary>
    public class Class {
        ///<summary>The CRN of the class, should be unique to each class</summary>
        [Key]
        public int CRN { get; set; }

        ///<summary>The department code of the class</summary>
        [Required]
        public int DepartmentCode { get; set; }

        ///<summary>The department of the class</summary>
        [ForeignKey (nameof (DepartmentCode))]
        public Department Department { get; set; }

        ///<summary>The name of the class ex: Intro to Computing</summary>
        [Required]
        public string Name { get; set; }

        ///<summary>The shortname of the class ex: CS101</summary>
        [Required]
        public string ShortName { get; set; }

        ///<summary>The sessionclasses this class has been used in</summary>
        [InverseProperty (nameof (SessionClass.Class))]
        public IEnumerable<SessionClass> SessionClasses { get; set; }
    }
}