namespace tcs_service.Models.DTO
{
    ///<summary>DTO represents the data used in the ReasonReport</summary>
    /// <remarks>
    /// This represents a person selecting a reason and a class during a signin.
    /// Each person the selects that specific reasons with that specific class increments the Visits count
    /// </remarks>
    public class ReasonWithClassVisitsDTO
    {
        ///<summary>The name of the reason</summary>
        public string ReasonName { get; set; }

        ///<summary>The id of the reason</summary>
        public int ReasonId { get; set; }

        ///<summary>The name of the class</summary>
        public string ClassName { get; set; }

        ///<summary>The CRN of the class</summary>
        public int ClassCRN { get; set; }

        ///<summary>The amount of people that choose this combination of Reason and Class during a SignIn</summary>
        public int Visits { get; set; }
    }
}
