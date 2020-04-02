using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tcs_service.Models {
    ///<summary>Represents a Department table</summary>
    public class Department {

        ///<summary>The Department code, unique to each department</summary>
        [Key]
        public int Code { get; set; }

        ///<summary>The name of the department</summary>
        [Required]
        public string Name { get; set; }

        ///<summary>The classes in this department</summary>
        [InverseProperty (nameof (Class.Department))]
        public List<Class> Classes { get; set; }
    }
}