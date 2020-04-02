namespace tcs_service.Models.DTO
{
    ///<summary>Represents the Data returned for the ClassTour Report</summary>
    public class ClassTourReportDTO
    {
        ///<summary>The Name of the ClassTour</summary>
        public string Name { get; set; }

        ///<summary>The total Number of Students that have come in for this classTour</summary>
        public int Students { get; set; }
    }
}
