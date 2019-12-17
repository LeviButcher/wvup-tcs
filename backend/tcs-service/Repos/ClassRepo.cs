using Microsoft.EntityFrameworkCore;
using System.Linq;
using tcs_service.Models;
using tcs_service.Repos.Base;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Repos
{
    public class ClassRepo : BaseRepo<Class>, IClassRepo
    {
        public ClassRepo(DbContextOptions options) : base(options) { }

        protected override IQueryable<Class> Include(DbSet<Class> set) => set.Include(x => x.Department);
    }
}
