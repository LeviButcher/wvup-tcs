using System.Linq;
using Microsoft.EntityFrameworkCore;
using tcs_service.Models;
using tcs_service.Repos.Base;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Repos {
    ///<summary>Repo for the Class Table</summary>
    public class ClassRepo : BaseRepo<Class>, IClassRepo {
        ///<summary>ClassRepo Constructor</summary>
        public ClassRepo (DbContextOptions options) : base (options) { }

        ///<summary>Include relational data during Get methods</summary>
        protected override IQueryable<Class> Include (DbSet<Class> set) => set.Include (x => x.Department);
    }
}