using Microsoft.EntityFrameworkCore;
using System.Linq;
using tcs_service.Models;
using tcs_service.Repos.Base;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Repos
{
    public class PersonRepo : BaseRepo<Person>, IPersonRepo
    {
        public PersonRepo(DbContextOptions options) : base(options) { }

        protected override IQueryable<Person> Include(DbSet<Person> set) => set;
    }
}
