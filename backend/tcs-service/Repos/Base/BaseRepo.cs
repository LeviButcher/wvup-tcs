using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using tcs_service.EF;

namespace tcs_service.Repos.Base {
    ///<summary>Methods that return data for a table from a database</summary>
    public abstract class BaseRepo<T> : IRepo<T> where T : class {

        ///<summary>The Database Context</summary>
        protected TCSContext _db;

        ///<summary>The specific table this repo is for</summary>
        protected DbSet<T> table;

        ///<summary>The BaseRepo Constructor</summary>
        protected BaseRepo () {
            _db = new TCSContext ();
            table = _db.Set<T> ();
        }

        ///<summary>The BaseRepo Constructor</summary>
        protected BaseRepo (TCSContext context) {
            _db = context;
            table = _db.Set<T> ();
        }

        ///<summary>The BaseRepo Constructor</summary>
        protected BaseRepo (DbContextOptions options) {
            _db = new TCSContext (options);
            table = _db.Set<T> ();
        }

        ///<summary>Create a new record in the table</summary>
        public async Task<T> Create (T t) {
            var added = await table.AddAsync (t);
            await SaveChangesAsync ();
            return added.Entity;
        }

        ///<summary>Create or update a record in the table depending on if a record is found with the function provided</summary>
        public async Task<T> CreateOrUpdate (Expression<Func<T, bool>> func, T t) {
            var item = await Find (func);
            if (item is T) {
                _db.Entry (item).State = EntityState.Detached;
                table.Update (t);
                await SaveChangesAsync ();
                return t;
            } else {
                return await Create (t);
            }
        }

        ///<summary>Create several new records in the table</summary>
        public async Task<int> Create (IEnumerable<T> tList) {
            await table.AddRangeAsync (tList);
            return await SaveChangesAsync ();
        }

        ///<summary>Update a record in the table</summary>
        public async Task<T> Update (T t) {
            var updated = table.Update (t);
            await SaveChangesAsync ();
            return updated.Entity;
        }

        ///<summary>Include relationship data into a result set</summary>
        virtual protected IQueryable<T> Include (DbSet<T> set) => set;

        ///<summary>Check if a record exists that matches the provided function</summary>
        public async Task<bool> Exist (Expression<Func<T, bool>> function) => await table.AnyAsync (function);

        ///<summary>Return the record that matches the provided function</summary>
        public async Task<T> Find (Expression<Func<T, bool>> function) => await Include (table).FirstOrDefaultAsync (function);

        ///<summary>Return all the records that matches the provided function</summary>
        public virtual IEnumerable<T> GetAll (Expression<Func<T, bool>> function) => Include (table).Where (function);

        ///<summary>Return all the records in the table</summary>
        public virtual IEnumerable<T> GetAll () => Include (table);

        ///<summary>Remove a record from the table</summary>
        public async virtual Task<T> Remove (Expression<Func<T, bool>> function) {
            var found = await Find (function);
            var deleted = table.Remove (found);
            await SaveChangesAsync ();
            return deleted.Entity;
        }

        ///<summary>Remove all records that match the provided function from the table</summary>
        public async Task<IEnumerable<T>> RemoveAll (Expression<Func<T, bool>> function) {
            var found = GetAll (function);
            table.RemoveRange (found);
            await SaveChangesAsync ();
            return found;
        }

        ///<summary>Save the local changes in the table to the database.</summary>
        /// Changes don't persit until SaveChangesIsCalled
        public async Task<int> SaveChangesAsync () {
            try {
                return await _db.SaveChangesAsync ();
            } catch (DbUpdateConcurrencyException ex) {
                Console.WriteLine (ex);
                throw;
            } catch (RetryLimitExceededException ex) {
                Console.WriteLine (ex);
                throw;
            } catch (Exception ex) {
                Console.WriteLine (ex);
                throw;
            }
        }
    }
}