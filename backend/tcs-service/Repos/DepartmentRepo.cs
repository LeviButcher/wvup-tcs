using Microsoft.EntityFrameworkCore;
using System.Linq;
using tcs_service.Models;
using tcs_service.Repos.Base;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Repos
{
    public class DepartmentRepo : BaseRepo<Department>, IDepartmentRepo
    {
        public DepartmentRepo(DbContextOptions options) : base(options) { }

        protected override IQueryable<Department> Include(DbSet<Department> set) => set;
    }
}
