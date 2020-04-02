using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using tcs_service.Models.Attributes;

namespace tcs_service.Models.DTOs {
    ///<summary>Data submitted during a POST request for creating a Session</summary>
    public class SessionPostOrPutDTO {
        ///<summary>Session Id</summary>
        public int Id { get; set; }

        ///<summary>WVUP Id of the person</summary>
        public int PersonId { get; set; }

        ///<summary>Time the session started</summary>
        [DataType (DataType.DateTime)]
        public DateTime InTime { get; set; }

        ///<summary>Time the session ended</summary>
        [DataType (DataType.DateTime)]
        [SignOutValidation ("InTime")]
        public DateTime OutTime { get; set; }

        ///<summary>The code of the semester of this session</summary>
        public int SemesterCode { get; set; }

        ///<summary>Wether or not the student had tutoring</summary>
        public bool Tutoring { get; set; }

        ///<summary>The classes the student selected</summary>
        public List<int> SelectedClasses { get; set; }

        ///<summary>The reasons the student selected</summary>
        public List<int> SelectedReasons { get; set; }
    }
}