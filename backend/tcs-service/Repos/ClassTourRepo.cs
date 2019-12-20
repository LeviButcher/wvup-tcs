using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using tcs_service.Models;
using tcs_service.Helpers;
using tcs_service.Repos.Base;
using tcs_service.Repos.Interfaces;
using tcs_service.Models.ViewModels;

namespace tcs_service.Repos
{
    public class ClassTourRepo : BaseRepo<ClassTour>, IClassTourRepo
    {

        public ClassTourRepo(DbContextOptions options) : base(options)
        {

        }

        // Just changing this for now, this will be removed with testing of the ClassTour Repo
        public Paging<ClassTourViewModel> GetBetweenDates(DateTime start, DateTime end, int skip, int take)
        {
            var tours = _db.ClassTours.Where(a => a.DayVisited > start && a.DayVisited < end).Select(x => new ClassTourViewModel()
            {
                Id = x.Id,
                Name = x.Name,
                DayVisited = x.DayVisited,
                NumberOfStudents = x.NumberOfStudents
            });

            var page = (int)Math.Ceiling((decimal)tours.Count() / take);

            return new Paging<ClassTourViewModel>(page, take, tours);
        }

        protected override IQueryable<ClassTour> Include(DbSet<ClassTour> set) => set;
    }
}