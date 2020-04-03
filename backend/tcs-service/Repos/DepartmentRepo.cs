using System.Linq;
using Microsoft.EntityFrameworkCore;
using tcs_service.Models;
using tcs_service.Repos.Base;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Repos {
    ///<summary>Repo for the Department Table</summary>
    public class DepartmentRepo : BaseRepo<Department>, IDepartmentRepo {
        ///<summary>DepartmentRepo Constructor</summary>
        public DepartmentRepo (DbContextOptions options) : base (options) { }
    }
}