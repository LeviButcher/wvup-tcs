using Microsoft.EntityFrameworkCore;
using System.Linq;
using tcs_service.Models;
using tcs_service.Repos.Base;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Repos
{
    public class SemesterRepo : BaseRepo<Semester>, ISemesterRepo
    {
        public SemesterRepo(DbContextOptions options) : base(options) { }

        protected override IQueryable<Semester> Include(DbSet<Semester> set) => set;
    }
}
