using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using tcs_service.Models;
using tcs_service.Repos.Base;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Repos
{
    public class SessionRepo : BaseRepo<Session>, ISessionRepo
    {
        public SessionRepo(DbContextOptions options) : base(options) { }

        protected override IQueryable<Session> Include(DbSet<Session> set) =>
            set.Include(x => x.SessionClasses).ThenInclude(x => x.Class)
            .Include(x => x.SessionReasons).ThenInclude(x => x.Reason)
            .Include(x => x.Person).ThenInclude(x => x.Schedules).ThenInclude(x => x.Class)
            .Include(x => x.Semester);

        public override IEnumerable<Session> GetAll(Expression<Func<Session, bool>> function) => base.GetAll(function).Where(x => x.Deleted == false);

        public async override Task<Session> Remove(Expression<Func<Session, bool>> function)
        {
            var found = await Find(function);
            found.Deleted = true;
            var updated = table.Update(found);
            await SaveChangesAsync();
            return updated.Entity;
        }
    }
}
