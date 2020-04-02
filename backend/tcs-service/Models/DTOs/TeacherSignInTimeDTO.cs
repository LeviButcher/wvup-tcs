namespace tcs_service.Models.DTO {
    ///<summary>Represents the data used in the Volunteer report</summary>
    public class TeacherSignInTimeDTO {
        ///<summary>The Email of the teacher</summary>
        public string TeacherEmail { get; set; }

        ///<summary>The FullName of the teacher</summary>
        public string FullName { get; set; }

        ///<summary>The TotalHours the teacher volunteered</summary>
        public decimal TotalHours { get; set; }
    }
}