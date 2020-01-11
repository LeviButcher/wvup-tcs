using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using tcs_service.Models;
using tcs_service.Repos.Base;
using tcs_service.Repos.Interfaces;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace tcs_service.Repos
{
    public class ClassTourRepo : BaseRepo<ClassTour>, IClassTourRepo
    {
        public ClassTourRepo(DbContextOptions options) : base(options) { } 

        protected override IQueryable<ClassTour> Include(DbSet<ClassTour> set) => set;

        public IEnumerable<ClassTour> GetBetweenDates(DateTime start, DateTime end)
        {
            var tours = _db.ClassTours.Where(a => a.DayVisited >= start && a.DayVisited <= end);
            
            return tours;
        }

        public override IEnumerable<ClassTour> GetAll(Expression<Func<ClassTour, bool>> function)
        {
            var tours = base.GetAll(function);
            return tours.Where(x => x.Deleted == false);
        }

        public override IEnumerable<ClassTour> GetAll()
        {
            var tours = base.GetAll();
            return tours.Where(x => x.Deleted == false);
        }

        public async override Task<ClassTour> Remove(Expression<Func<ClassTour, bool>> function)
        {
            var found = await Find(function);
            found.Deleted = true;
            var updated = table.Update(found);
            await SaveChangesAsync();
            return updated.Entity;
        }
    }
}