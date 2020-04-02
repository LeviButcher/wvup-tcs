using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using tcs_service.Models.Attributes;

namespace tcs_service.Models.DTOs {
    ///<summary>SessionInfoDTO</summary>
    public class SessionInfoDTO : SessionPostOrPutDTO {
        ///<summary>The time the session ended</summary>
        [DataType (DataType.DateTime)]
        [SignOutValidation ("InTime")]
        new public DateTime? OutTime { get; set; }

        ///<summary>The time the session ended</summary>
        public PersonType PersonType { get; set; }

        ///<summary>The Email of the person</summary>
        public string Email { get; set; }

        ///<summary>The schedule of the person</summary>
        public IEnumerable<Class> Schedule { get; set; }

        ///<summary>The FirstName of the person</summary>
        public string FirstName { get; set; }

        ///<summary>The LastName of the person</summary>
        public string LastName { get; set; }
    }
}