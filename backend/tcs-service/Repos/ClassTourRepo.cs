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

        public async Task<ClassTour> Add(ClassTour tour)
        {
            await _db.AddAsync(tour);
            await _db.SaveChangesAsync();
            return tour;
        }


        public async Task<Paging<ClassTourViewModel>> GetBetweenDates(DateTime start, DateTime end, int skip, int take)
        {
            var tours = _db.ClassTours.Where(a => a.DayVisited > start && a.DayVisited < end);
            var totalDataCount = await tours.CountAsync();
            var pageData = GetPageData(tours, skip, take);

            return new Paging<ClassTourViewModel>(skip, take, pageData);
        }

        protected override IQueryable<ClassTour> Include(DbSet<ClassTour> set) => set;

        private IQueryable<ClassTourViewModel> GetPageData(IQueryable<ClassTour> tours, int skip, int take)
        {
            return tours
                .Skip(skip).Take(take)
                .Select(x => new ClassTourViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    DayVisited = x.DayVisited,
                    NumberOfStudents = x.NumberOfStudents
                });
        }
    }
}