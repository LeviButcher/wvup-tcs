using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using tcs_service.Models;
using tcs_service.Repos.Base;
using tcs_service.Repos.Interfaces;

namespace tcs_service.Repos {
    ///<summary>Repo for the ClassTour Table</summary>
    public class ClassTourRepo : BaseRepo<ClassTour>, IClassTourRepo {
        ///<summary>ClassTourRepo Constructor</summary>
        public ClassTourRepo (DbContextOptions options) : base (options) { }

        ///<summary>Return all records in ClassTour table matching the provided function</summary>
        public override IEnumerable<ClassTour> GetAll (Expression<Func<ClassTour, bool>> function) {
            var tours = base.GetAll (function);
            return tours.Where (x => x.Deleted == false);
        }

        ///<summary>Return all records in ClassTour table</summary>
        public override IEnumerable<ClassTour> GetAll () {
            var tours = base.GetAll ();
            return tours.Where (x => x.Deleted == false);
        }

        ///<summary>Delete a record from the Class Tour Table, only sets Delete to true</summary>
        public async override Task<ClassTour> Remove (Expression<Func<ClassTour, bool>> function) {
            var found = await Find (function);
            found.Deleted = true;
            var updated = table.Update (found);
            await SaveChangesAsync ();
            return updated.Entity;
        }
    }
}