using Microsoft.EntityFrameworkCore;
using System.Linq;
using tcs_service.Models;
using tcs_service.Repos.Base;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Repos
{
    public class SessionReasonRepo : BaseRepo<SessionReason>, ISessionReasonRepo
    {
        public SessionReasonRepo(DbContextOptions options) : base(options) { }

        protected override IQueryable<SessionReason> Include(DbSet<SessionReason> set) => set;
    }
}
