using System.Collections.Generic;

namespace tcs_service.Models.ViewModels
{
    class BannerInformation
    {
        public int WVUPID { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<Course> Courses { get; set; }
        public int TermCode { get; set; }
    }
}
