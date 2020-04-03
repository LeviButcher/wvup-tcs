using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using tcs_service.Models;
using tcs_service.Repos.Base;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Repos {
    ///<summary>Repo for the Session Table</summary>
    public class SessionRepo : BaseRepo<Session>, ISessionRepo {

        ///<summary>SessionRepo Constructor</summary>
        public SessionRepo (DbContextOptions options) : base (options) { }

        ///<summary>Include relation data for Get methods</summary>
        protected override IQueryable<Session> Include (DbSet<Session> set) =>
            set.Include (x => x.SessionClasses).ThenInclude (x => x.Class)
            .Include (x => x.SessionReasons).ThenInclude (x => x.Reason)
            .Include (x => x.Person).ThenInclude (x => x.Schedules).ThenInclude (x => x.Class)
            .Include (x => x.Semester);

        ///<summary>Return all the records in the table that are not deleted and match the provided function</summary>
        public override IEnumerable<Session> GetAll (Expression<Func<Session, bool>> function) {
            var sessions = base.GetAll (function);
            return sessions.Where (x => x.Deleted == false);
        }

        ///<summary>Return all records in the table that are not deleted</summary>
        public override IEnumerable<Session> GetAll () {
            var sessions = base.GetAll ();
            return sessions.Where (x => x.Deleted == false);
        }

        ///<summary>Remove a record form the table that matches the provided function, set it's deleted to true</summary>
        public async override Task<Session> Remove (Expression<Func<Session, bool>> function) {
            var found = await Find (function);
            found.Deleted = true;
            var updated = table.Update (found);
            await SaveChangesAsync ();
            return updated.Entity;
        }
    }
}