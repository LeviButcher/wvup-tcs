using Microsoft.EntityFrameworkCore;
using System.Linq;
using tcs_service.Models;
using tcs_service.Repos.Base;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Repos
{
    public class SessionClassRepo : BaseRepo<SessionClass>, ISessionClassRepo
    {
        public SessionClassRepo(DbContextOptions options) : base(options) { }

        protected override IQueryable<SessionClass> Include(DbSet<SessionClass> set) => set;
    }
}
