using Microsoft.EntityFrameworkCore;
using System.Linq;
using tcs_service.Models;
using tcs_service.Repos.Base;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Repos
{
    public class ScheduleRepo : BaseRepo<Schedule>, IScheduleRepo
    {
        public ScheduleRepo(DbContextOptions options) : base(options) { }

        protected override IQueryable<Schedule> Include(DbSet<Schedule> set) => set.Include(x => x.Course).ThenInclude(x => x.Department);
    }
}
