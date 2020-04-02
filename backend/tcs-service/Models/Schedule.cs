using System.ComponentModel.DataAnnotations.Schema;

namespace tcs_service.Models {
    ///<summary>Represents the Schedule table</summary>
    /// This table represents the classes that a student has
    /// Every class a student has ever had should be stored here
    public class Schedule {
        ///<summary>The WVUP Id of the person</summary>
        public int PersonId { get; set; }

        ///<summary>The person this schedule is for</summary>
        [ForeignKey (nameof (PersonId))]
        public Person Person { get; set; }

        ///<summary>The CRN of the class the student has</summary>
        public int ClassCRN { get; set; }

        ///<summary>The actual class the student has</summary>
        [ForeignKey (nameof (ClassCRN))]
        public Class Class { get; set; }

        ///<summary>The semester the student had this class</summary>
        public int SemesterCode { get; set; }

        ///<summary>The actual semester the student had this class</summary>
        [ForeignKey (nameof (SemesterCode))]
        public Semester Semester { get; set; }

    }
}