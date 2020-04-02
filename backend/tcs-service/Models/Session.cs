using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using tcs_service.Models.Attributes;

namespace tcs_service.Models {
    ///<summary>Represents the Session Table</summary>
    public class Session {

        ///<summary>The Id of this session</summary>
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        ///<summary>The WVUP Id of this person</summary>
        [Required]
        public int PersonId { get; set; }

        ///<summary>The actual Person</summary>
        [ForeignKey (nameof (PersonId))]
        public Person Person { get; set; }

        ///<summary>The time this session started</summary>
        [Required]
        public DateTime InTime { get; set; }

        ///<summary>The time this session ended</summary>
        [SignOutValidation ("InTime")]
        public DateTime? OutTime { get; set; }

        ///<summary>if the student was here for tutoring</summary>
        public bool Tutoring { get; set; }

        ///<summary>The classes the student is here for</summary>
        [InverseProperty (nameof (SessionClass.Session))]
        public IEnumerable<SessionClass> SessionClasses { get; set; } = new List<SessionClass> ();

        ///<summary>The reasons the student is here for</summary>
        [InverseProperty (nameof (SessionReason.Session))]
        public IEnumerable<SessionReason> SessionReasons { get; set; } = new List<SessionReason> ();

        ///<summary>The semester code of when this session took place</summary>
        public int SemesterCode { get; set; }

        ///<summary>The actual Semester</summary>
        [ForeignKey (nameof (SemesterCode))]
        public Semester Semester { get; set; }

        ///<summary>If this session has been deleted</summary>
        public bool Deleted { get; set; }

    }
}