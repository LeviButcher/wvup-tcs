using System.ComponentModel.DataAnnotations;

namespace tcs_service.Models {
    ///<summary>Represents the User table</summary>
    /// Users are accounts that can log in and view records
    /// Only meant for TCS Employees
    public class User {
        ///<summary>The unique Id of the user</summary>
        public int Id { get; set; }

        ///<summary>The firstname of the user</summary>
        public string FirstName { get; set; }

        ///<summary>The lastnames of the user</summary>
        public string LastName { get; set; }

        ///<summary>The Username of the user, must be unique</summary>
        [Required (ErrorMessage = "Username is required")]
        [StringLength (25, MinimumLength = 3, ErrorMessage = "Username should be at least 3 character and maximum of 25")]
        public string Username { get; set; }

        ///<summary>The Password Hash of the user</summary>
        public byte[] PasswordHash { get; set; }

        ///<summary>The Password Salt used for Password</summary>
        public byte[] PasswordSalt { get; set; }

        ///<summary>Copy all the properties of a user to a new user</summary>
        public User Copy () {
            return (User) this.MemberwiseClone ();
        }
    }
}