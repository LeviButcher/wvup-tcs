using Microsoft.EntityFrameworkCore;
using System.Linq;
using tcs_service.Models;
using tcs_service.Repos.Base;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Repos
{
    public class ReasonRepo : BaseRepo<Reason>, IReasonRepo
    {
        public ReasonRepo(DbContextOptions options) : base(options) { }

        protected override IQueryable<Reason> Include(DbSet<Reason> set) => set;
    }
}
