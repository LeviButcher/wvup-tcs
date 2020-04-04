using System.Collections.Generic;

namespace tcs_service.Models.DTOs
{
    ///<summary>Represents the general info about a person, such as their type and schedule</summary>
    public class PersonInfoDTO
    {
        ///<summary>The WVUP Id of the person</summary>
        public int Id { get; set; }

        ///<summary>The WVUP Email of the person</summary>
        public string Email { get; set; }

        ///<summary>The FirstName of the person</summary>
        public string FirstName { get; set; }

        ///<summary>The LastName of the person</summary>
        public string LastName { get; set; }

        ///<summary>The Schedule of the person</summary>
        public IEnumerable<Class> Schedule { get; set; } = new List<Class>();

        ///<summary>The PersonType of the person</summary>
        public PersonType PersonType { get; set; }
    }
}
