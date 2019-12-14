using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using tcs_service.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace tcs_service.Repos.Base
{
    public abstract class BaseRepo<T> : IRepo<T> where T : class
    {
        protected TCSContext _db;
        protected DbSet<T> table;

        protected BaseRepo()
        {
            _db = new TCSContext();
            table = _db.Set<T>();
        }

        protected BaseRepo(TCSContext context)
        {
            _db = context;
            table = _db.Set<T>();
        }

        protected BaseRepo(DbContextOptions options)
        {
            _db = new TCSContext(options);
            table = _db.Set<T>();
        }

        public async Task<T> Create(T t)
        {
            var added = await table.AddAsync(t);
            await SaveChangesAsync();
            return added.Entity;
        }

        public async Task<T> CreateOrUpdate(Expression<Func<T, bool>> func, T t)
        {
            var exists = await Exist(func);
            if (exists)
            {
                table.Update(t);
                await SaveChangesAsync();
                return t;
            }
            else
            {
                return await Create(t);
            }
        }

        public async Task<T> Update(T t)
        {
            var updated = table.Update(t);
            await SaveChangesAsync();
            return updated.Entity;
        }

        protected abstract IQueryable<T> Include(DbSet<T> set);

        public async Task<bool> Exist(Expression<Func<T, bool>> function) => await table.AnyAsync(function);

        public async Task<T> Find(Expression<Func<T, bool>> function)
            => await Include(table).FirstOrDefaultAsync(function);

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> function) => Include(table).Where(function);

        public IEnumerable<T> GetAll() => Include(table);

        public async Task<T> Remove(Expression<Func<T, bool>> function)
        {
            var found = await Find(function);
            var deleted = table.Remove(found);
            await SaveChangesAsync();
            return deleted.Entity;
        }

        public async Task<IEnumerable<T>> RemoveAll(Expression<Func<T, bool>> function)
        {
            var found = GetAll(function);
            table.RemoveRange(found);
            await SaveChangesAsync();
            return found;
        }

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            catch (RetryLimitExceededException ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}