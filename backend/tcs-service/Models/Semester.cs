using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tcs_service.Models
{
    public class Semester
    {
        [Key]
        public int Code { get; set; }

        [NotMapped]
        public string Name
        {
            get
            {
                string year = Code.ToString().Substring(0, 4);
                string partOfYear = Code.ToString().Substring(4);
                switch (partOfYear)
                {
                    case "01":
                        return $"Spring {year}";
                    case "02":
                        return $"Summer {year}";
                    case "03":
                        return $"Fall {year}";
                }
                return Code.ToString();
            }
        }
    }
}
