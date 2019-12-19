using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using tcs_service.Models;
using tcs_service.Repos.Base;
using tcs_service.Repos.Interfaces;
using System.Collections.Generic;


namespace tcs_service.Repos
{
    public class ClassTourRepo : BaseRepo<ClassTour>, IClassTourRepo
    {

        public ClassTourRepo(DbContextOptions options) : base(options) { } 

        protected override IQueryable<ClassTour> Include(DbSet<ClassTour> set) => set;

        public IEnumerable<ClassTour> GetBetweenDates(DateTime start, DateTime end)
        {
            var tours = _db.ClassTours.Where(a => a.DayVisited > start && a.DayVisited < end);
            
            return tours;
        }
    }
}