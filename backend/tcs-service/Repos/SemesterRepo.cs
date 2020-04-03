using System.Linq;
using Microsoft.EntityFrameworkCore;
using tcs_service.Models;
using tcs_service.Repos.Base;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Repos {
    ///<summary>Repo for the Semester Table</summary>
    public class SemesterRepo : BaseRepo<Semester>, ISemesterRepo {

        ///<summary>SemesterRepo Constructor</summary>
        public SemesterRepo (DbContextOptions options) : base (options) { }
    }
}