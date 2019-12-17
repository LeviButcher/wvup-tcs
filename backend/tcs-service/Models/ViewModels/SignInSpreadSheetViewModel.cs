using System.Linq;
using System;
using System.Collections.Generic;

namespace tcs_service.Models.ViewModels
{
    public class SignInSpreadSheetViewModel
    {
        public SignInSpreadSheetViewModel(PersonType type, IEnumerable<Class> classes, IEnumerable<Reason> reasons, bool tutoring)
        {
            IsTeacher = type.Equals(PersonType.Teacher);
            if (classes.Count() > 0)
                ClassesVisitingFor = classes.Select(x => x.ShortName).Aggregate((acc, curr) => acc + ", " + curr);
            if (tutoring)
                ReasonsForVisiting += "Tutoring";
            if (reasons.Count() > 0)
                ReasonsForVisiting = reasons.Select(x => x.Name).Append(ReasonsForVisiting).Where(x => !String.IsNullOrEmpty(x)).Reverse().Aggregate((acc, curr) => acc + ", " + curr);
        }



        public int WVUPId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset? InTime { get; set; }
        public DateTimeOffset? OutTime { get; set; }
        public string ReasonsForVisiting { get; }
        public string ClassesVisitingFor { get; }
        public bool IsTeacher { get; }
    }
}