using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tcs_service.EF;
using tcs_service.Models;
using tcs_service.Repos.Base;
using tcs_service.Repos.Interfaces;

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

        public async override Task<bool> Exist(int id)
        {
            return await _db.ClassTours.AnyAsync(e => e.ID == id);
        }

        public async override Task<ClassTour> Find(int id)
        {
            return await _db.ClassTours.SingleOrDefaultAsync(a => a.ID == id);
        }

        public override IEnumerable<ClassTour> GetAll()
        {
            return _db.ClassTours;
        }

        public async Task<IEnumerable<ClassTour>> GetBetweenDates(DateTime start, DateTime end)
        {
            
            var tours =  _db.ClassTours.Where(a => a.DayVisited > start && a.DayVisited < end);

            //var tour = _db.ClassTours.Where(p =>  == id)
            //   .GroupBy(x => x.PersonId)
            //   .Select(e => e.OrderByDescending(t => t.InTime)).FirstOrDefault();

           // result.Add(await _db.ClassTours.FirstOrDefaultAsync(a => a.DayVisited > start && a.DayVisited < end));
            
            return tours;
        }

        public async override Task<ClassTour> Remove(int id)
        {
            var tour = await _db.ClassTours.SingleAsync(a => a.ID == id);
            _db.ClassTours.Remove(tour);
            await _db.SaveChangesAsync();
            return tour;
        }

        public async Task<ClassTour> Update(ClassTour tour)
        {
            _db.ClassTours.Update(tour);
            await _db.SaveChangesAsync();
            return tour;
        }
    }
}