namespace tcs_service.Models.DTOs {
    ///<summary>Represents a User. Used during Updating Users or Getting their data</summary>
    public class UserDto {
        ///<summary>Id of the User</summary>
        public int Id { get; set; }

        ///<summary>Username of the User</summary>
        public string Username { get; set; }

        ///<summary>FirstName of the User</summary>
        public string FirstName { get; set; }

        ///<summary>LastName of the User</summary>
        public string LastName { get; set; }

        ///<summary>The JWT Token that authenticates the user's session</summary>
        public string Token { get; set; }

        ///<summary>The Password the user would like to set, Should only be passed in to REST API, never returned</summary>
        public string Password { get; set; }

    }
}