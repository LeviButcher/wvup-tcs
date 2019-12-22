using System;
using System.Linq;
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
    }
}