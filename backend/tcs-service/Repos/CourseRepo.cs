using Microsoft.EntityFrameworkCore;
using System.Linq;
using tcs_service.Models;
using tcs_service.Repos.Base;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Repos
{
    public class CourseRepo : BaseRepo<Course>, ICourseRepo
    {
        public CourseRepo(DbContextOptions options) : base(options) { }

        protected override IQueryable<Course> Include(DbSet<Course> set) => set;
    }
}
