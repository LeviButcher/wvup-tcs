using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using CsvHelper.Configuration;

namespace tcs_service.Helpers
{
    ///<summary>Represents a record in a CSVSessionUpload</summary>
    /// Students should have CRNs, Tutoring, and Reasons
    /// Teacher records do not have too
    public class CSVSessionUpload
    {
        ///<summary>WVUP Email of Student or Teacher</summary>
        [Required]
        public string Email { get; set; }

        ///<summary>The Time the Student or Teacher came in</summary>
        [Required]
        public DateTime InTime { get; set; }

        ///<summary>The Time the Student or Teacher left</summary>
        [Required]
        public DateTime OutTime { get; set; }

        ///<summary>Wether or not the student was here for tutoring</summary>
        [Required]
        public bool Tutoring { get; set; }

        ///<summary>The CRNs of the Classes the Student was here for</summary>
        public IEnumerable<int> CRNs { get; set; } = new List<int>();

        ///<summary>The Reasons the Student was here for</summary>
        public IEnumerable<string> Reasons { get; set; } = new List<string>();

        ///<summary>The SemesterCode of the Semester this Session took place in</summary>
        [Required]
        public int SemesterCode { get; set; }
    }

    ///<summary>CSVSessionUploadMap</summary>
    public sealed class CSVSessionUploadMap : ClassMap<CSVSessionUpload>
    {

        /// CSVSessionUploadMap
        public CSVSessionUploadMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.CRNs).ConvertUsing(row =>
              row.GetField<string>("CRNs")
              .Split(",").Select(x => x.Trim())
              .Select(x => int.Parse(x)));
            Map(m => m.Reasons).ConvertUsing(row => row.GetField("Reasons").Split(",").Select(x => x.Trim()));
        }
    }
}