using System.Linq;
using Microsoft.EntityFrameworkCore;
using tcs_service.Models;
using tcs_service.Repos.Base;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Repos {
    ///<summary>Repo for the Schedule Table</summary>
    public class ScheduleRepo : BaseRepo<Schedule>, IScheduleRepo {
        ///<summary>ScheduleTable constructor</summary>
        public ScheduleRepo (DbContextOptions options) : base (options) { }

        ///<summary>Include relational data on Get Methods</summary>
        protected override IQueryable<Schedule> Include (DbSet<Schedule> set) => set.Include (x => x.Class).ThenInclude (x => x.Department);
    }
}