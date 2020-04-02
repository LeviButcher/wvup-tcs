using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tcs_service.Models {
    ///<summary>Represents the Semester table</summary>
    public class Semester {
        ///<summary>the semester code, ex:201901</summary>
        /// <remarks>
        /// format of the code is as follows
        /// first four digits is year
        /// last two digits is what semester this semester code is in
        /// 01 -> Fall
        /// 02 -> Spring
        /// 03 -> Summer
        /// 201901 is Fall of 2018
        /// </remarks>
        [Key]
        public int Code { get; set; }

        ///<summary>Get a user friendly representation of the year</summary>
        [NotMapped]
        public string Name {
            get {
                var year = int.Parse (Code.ToString ().Substring (0, 4));
                string partOfYear = Code.ToString ().Substring (4);
                switch (partOfYear) {
                    case "01":
                        return $"Fall {year - 1}";
                    case "02":
                        return $"Spring {year}";
                    case "03":
                        return $"Summer {year}";
                }
                return Code.ToString ();
            }
        }
    }
}